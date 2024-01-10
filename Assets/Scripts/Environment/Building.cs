using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

[RequireComponent(typeof(RefreshBuildingTimer))]
public class Building : MonoBehaviour
{
    [Serializable]
    private class LevelState
    {
        public int RequiredXp;
        public GameObject[] ObjectsToActive;

        public bool ObjectsWasActived { get; set; }
    }

    [Serializable]
    public class SavedData
    {
        public int ID;
        public int Level;
        public int CurrentXP;
        public int SecondsBeforeUpdate;
        public bool ShouldBeRefreshed;
        public Door.SavedData[] DoorLockStates;
        public Lootable.SavedData[] LootableEmptyStates;
    }

    public static event Action<bool, Building> PlayerInBuilding;
    public static event Action PlayerInBuildingWithEnemyFirstly;
    public static event Action StatsChanged;

    [SerializeField] private bool _enableUpdates = true;
    [SerializeField] private EnemyAI[] _enemies;
    [SerializeField] private Door[] _doors;
    [SerializeField] private Lootable[] _lootables;
    [SerializeField] private LevelState[] _levelStates;

    private int _level = 1;
    private int _currentXp;
    private int _requiredXp;
    private bool _triggered;
    private bool _isIntersectTriggers;
    private bool _shouldBeRefreshed;
    private RefreshBuildingTimer _refreshTimer;
    private readonly SavedData _savedData = new();

    private bool IsMaxLevel => _level > _levelStates.Length;

    public SavedData Save()
    {
        _savedData.ID = GetHashCode();
        _savedData.Level = _level;
        _savedData.CurrentXP = _currentXp;
        _savedData.SecondsBeforeUpdate = _refreshTimer.RemainSeconds;
        _savedData.ShouldBeRefreshed = _shouldBeRefreshed;
        _savedData.DoorLockStates = _doors.Select(door => door.Save()).ToArray();
        _savedData.LootableEmptyStates = _lootables.Select(lootable => lootable.Save()).ToArray();
        return _savedData;
    }

    public void Load(SavedData data)
    {
        _level = data.Level;
        _currentXp = data.CurrentXP;
        _refreshTimer.RemainSeconds = data.SecondsBeforeUpdate;
        _shouldBeRefreshed = data.ShouldBeRefreshed;

        var doorLockStates = data.DoorLockStates.ToDictionary(door => door.ID);
        foreach (var door in _doors)
            door.Load(doorLockStates[door.GetHashCode()]);

        var lootableEmptyStates = data.LootableEmptyStates.ToDictionary(lootable => lootable.ID);
        foreach (var lootable in _lootables)
            lootable.Load(lootableEmptyStates[lootable.GetHashCode()]);
    }

    public void Initialize()
    {
        for (var i = 0; i < _level - 1; i++)
            ApplyLevelChanges(_levelStates[i]);

        if (!IsMaxLevel)
            _requiredXp = _levelStates[_level - 1].RequiredXp;

        if (_refreshTimer != null && _shouldBeRefreshed)
            _refreshTimer.StartTimer();

        UpdateProgressBar();
        UpdateLevelText();
    }

    public void OnPlayerEnter()
    {
        if (_triggered)
        {
            _isIntersectTriggers = true;
            return;
        }
        _triggered = true;

        PlayerInBuilding?.Invoke(true, this);

        if (_enableUpdates)
            LockDoors(false);

        if (_refreshTimer != null)
            _refreshTimer.StopTimer();
    }

    public void OnPlayerExit()
    {
        if (_isIntersectTriggers)
        {
            _isIntersectTriggers = false;
            return;
        }
        _triggered = false;

        PlayerInBuilding?.Invoke(false, this);


        var doorsLocked = false;
        foreach (var enemy in _enemies)
        {
            if (_enableUpdates && !doorsLocked && enemy.Worried)
            {               
                LockDoors(true);
                doorsLocked = true;
            }
            enemy.Calm();
        }

        if (_refreshTimer != null)
            _refreshTimer.StartTimer();
    }

    public void LockDoors(bool value)
    {
        foreach (var door in _doors)
        {
            door.Lock(value);
            if (value == true)
                door.Close();
        }
    }

    public bool ContainsEnemies()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                if (!YandexGame.savesData.TutorialEnterBuildingWithEnemyDone)
                    PlayerInBuildingWithEnemyFirstly?.Invoke();
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        if (TryGetComponent<RefreshBuildingTimer>(out _refreshTimer))
            _refreshTimer.Initialize(Refresh, UpdateTimerText);

        void OnLooted()
        {
            AddXp();
            _shouldBeRefreshed = true;
        }
        foreach (var lootable in _lootables)
            lootable.Initialize(OnLooted);

        foreach (var enemy in _enemies)
            enemy.Initialize(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out MovementController player))
            OnPlayerEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out MovementController player))
            OnPlayerExit();
    }

    private void Refresh()
    {
        LockDoors(true);
        FillContainers();

        if (_currentXp >= _requiredXp)
            NextLevel();

        if (_level > 1 && !_levelStates[_level - 2].ObjectsWasActived)
            ApplyLevelChanges(_levelStates[_level - 2]);

        _shouldBeRefreshed = false;
        StatsChanged?.Invoke();
    }

    private void ApplyLevelChanges(LevelState levelState)
    {
        foreach (var obj in levelState.ObjectsToActive)
            obj.SetActive(true);
        levelState.ObjectsWasActived = true;
    }

    private void FillContainers()
    {
        foreach (var lootable in _lootables)
            lootable.SetEmpty(false);
    }

    private void AddXp()
    {
        ++_currentXp;
        UpdateProgressBar();
        StatsChanged?.Invoke();
    }

    private void NextLevel()
    {
        if (!IsMaxLevel)
        {
            ++_level;
            UpdateLevelText();

            if (!IsMaxLevel)
            {
                _currentXp = 0;
                _requiredXp = _levelStates[_level - 1].RequiredXp;
            }
            UpdateProgressBar();
        }
    }

    private void UpdateProgressBar()
    {
        string valueText = null;
        if (!IsMaxLevel && _currentXp >= _requiredXp)
            valueText = Translation.GetCompleteName();

        foreach (var door in _doors)
            door.SetProgressBarValue(_currentXp, _requiredXp, valueText);
    }

    private void UpdateLevelText()
    {
        foreach (var door in _doors)
            door.SetBuildingLevel(_level);
    }

    private void UpdateTimerText(int seconds)
    {
        foreach (var door in _doors)
            door.SetTimerText(seconds);
    }
}
