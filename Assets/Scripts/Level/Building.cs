using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

[RequireComponent(typeof(RefreshBuildingTimer), typeof(TriggerZone), typeof(BoxCollider))]
public class Building : MonoBehaviour, IIdentifiable
{
    public static event Action<bool, Building> GPlayerInBuilding;
    public static event Action PlayerInBuildingWithEnemyFirstly;
    public static event Action StatsChanged;

    [SerializeField] private bool _enableUpdates = true;
    [SerializeField] private bool _enableIndoorCamera = true;
    [SerializeField] private LevelState[] _levelStates;

    private HumanAI[] _enemies;
    private Door[] _doors;
    private Lootable[] _lootables;
    private TriggerZone _triggerZone;
    private int _level = 1;
    private int _currentXp;
    private int _requiredXp;
    private bool _triggered;
    private bool _isIntersectTriggers;
    private bool _shouldBeRefreshed;
    private RefreshBuildingTimer _refreshTimer;
    private readonly SavedData _savedData = new();

    private bool IsMaxLevel => _level > _levelStates.Length;

    public bool IndoorCameraEnabled => _enableIndoorCamera;
    public int ID { get; set; }

    private void Awake()
    {
        if (TryGetComponent<RefreshBuildingTimer>(out _refreshTimer))
            _refreshTimer.Initialize(Refresh, UpdateTimerText);

        _lootables = GetComponentsInChildren<Lootable>(true);
        _doors = GetComponentsInChildren<Door>(true);
        _enemies = GetComponentsInChildren<HumanAI>(true);
        _triggerZone = GetComponent<TriggerZone>();

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

    private void OnEnable()
    {
        _triggerZone.SubscribeOnEnter(OnPlayerEnter);
        _triggerZone.SubscribeOnExit(OnPlayerExit);
    }

    private void OnDisable()
    {
        _triggerZone.UnsubscribeOnEnter(OnPlayerEnter);
        _triggerZone.UnsubscribeOnExit(OnPlayerExit);
    }

    public SavedData Save()
    {
        _savedData.ID = ID;
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
            door.Load(doorLockStates[door.ID]);

        var lootableEmptyStates = data.LootableEmptyStates.ToDictionary(lootable => lootable.ID);
        foreach (var lootable in _lootables)
            lootable.Load(lootableEmptyStates[lootable.ID]);
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

    public IEnumerable<IIdentifiable> GetIdentifiables() => _lootables.Cast<IIdentifiable>().Concat(_doors);

    public void OnPlayerEnter(MovementController player)
    {
        if (_triggered)
        {
            _isIntersectTriggers = true;
            return;
        }
        _triggered = true;

        GPlayerInBuilding?.Invoke(true, this);

        if (_enableUpdates)
            LockDoors(false);

        if (_refreshTimer != null)
            _refreshTimer.StopTimer();
    }

    public void OnPlayerExit(MovementController player)
    {
        if (_isIntersectTriggers)
        {
            _isIntersectTriggers = false;
            return;
        }
        _triggered = false;

        GPlayerInBuilding?.Invoke(false, this);


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

    public bool ContainsWorriedEnemies()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.Worried)
                return true;
        }
        return false;
    }

    public void NextLevelDebug()
    {
        NextLevel();
        if (_level > 1 && !_levelStates[_level - 2].ObjectsWasActived)
            ApplyLevelChanges(_levelStates[_level - 2]);
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
        foreach (var obj in levelState.ObjectsToInactive)
            obj.SetActive(false);
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
            MetricaSender.BuildingUpgrade(gameObject.name, _level);
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

    [Serializable]
    private class LevelState
    {
        public int RequiredXp;
        public GameObject[] ObjectsToActive;
        public GameObject[] ObjectsToInactive;

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
}
