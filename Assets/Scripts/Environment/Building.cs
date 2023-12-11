using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Serializable]
    private class LevelState
    {
        public int RequiredXp;
        public GameObject[] ObjectsToActive;

        public bool ObjectsWasActived { get; set; }
    }

    public static event Action<bool> PlayerInBuilding;

    [SerializeField] private EnemyAI[] _enemies;
    [SerializeField] private Door[] _doors;
    [SerializeField] private Lootable[] _lootables;
    [SerializeField] private LevelState[] _levelStates;

    private bool IsMaxLevel => _level > _levelStates.Length;

    private int _level = 1;
    private int _currentXp;
    private int _requiredXp;
    private RefreshBuildingTimer _refreshTimer;

    public void OnPlayerEnter()
    {
        PlayerInBuilding?.Invoke(true);
        LockDoors(false);

        if (_refreshTimer != null)
            _refreshTimer.StopTimer();
    }

    public void OnPlayerExit()
    {
        PlayerInBuilding?.Invoke(false);

        var doorsLocked = false;
        foreach (var enemy in _enemies)
        {
            if (!doorsLocked && enemy.Worried)
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

    private void Awake()
    {
        if (TryGetComponent<RefreshBuildingTimer>(out _refreshTimer))
            _refreshTimer.Initialize(Refresh, UpdateTimerText);

        foreach (var lootable in _lootables)
            lootable.Initialize(AddXp);

        foreach (var enemy in _enemies)
            enemy.Initialize(this);

        if (!IsMaxLevel)
            _requiredXp = _levelStates[_level - 1].RequiredXp;
        UpdateProgressBar();
    }

    private void Refresh()
    {
        LockDoors(true);
        FillContainers();

        if (_currentXp >= _requiredXp)
            NextLevel();

        if (_level > 1 && !_levelStates[_level - 2].ObjectsWasActived)
        {
            foreach (var obj in _levelStates[_level - 2].ObjectsToActive)
                obj.SetActive(true);
            _levelStates[_level - 2].ObjectsWasActived = true;
        }
    }

    private void FillContainers()
    {
        foreach (var lootable in _lootables)
            lootable.Fill();
    }

    private void UpdateTimerText(int seconds)
    {
        foreach (var door in _doors)
            door.SetTimerText(seconds);
    }

    private void AddXp()
    {
        ++_currentXp;
        UpdateProgressBar();
    }

    private void NextLevel()
    {
        if (!IsMaxLevel)
        {
            ++_level;
            foreach (var door in _doors)
                door.SetBuildingLevel(_level);

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
}
