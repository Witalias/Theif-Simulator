using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    public static event Action<bool> PlayerInBuilding;

    [SerializeField] private EnemyAI[] _enemies;
    [SerializeField] private Door[] _doors;
    [SerializeField] private Lootable[] _lootables;

    private RefreshBuildingTimer _refreshTimer;

    public void OnPlayerEnter()
    {
        PlayerInBuilding?.Invoke(true);

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
                LockDoors();
                doorsLocked = true;
            }
            enemy.Calm();
        }

        if (_refreshTimer != null)
            _refreshTimer.StartTimer();
    }

    public void LockDoors()
    {
        foreach (var door in _doors)
            door.Lock(true);
    }

    private void Awake()
    {
        if (TryGetComponent<RefreshBuildingTimer>(out _refreshTimer))
            _refreshTimer.Initialize(Refresh);
    }

    private void Start()
    {
        foreach (var enemy in _enemies)
            enemy.Initialize(this);
    }

    private void Refresh()
    {
        LockDoors();
        FillContainers();
    }

    private void FillContainers()
    {
        foreach (var lootable in _lootables)
            lootable.Fill();
    }
}
