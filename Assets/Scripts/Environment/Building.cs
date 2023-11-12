using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    public static event Action<bool> PlayerInBuilding;

    [SerializeField] private EnemyAI[] _enemies;
    [SerializeField] private Door[] _doors;

    public void OnPlayerEnter()
    {
        PlayerInBuilding?.Invoke(true);
    }

    public void OnPlayerExit()
    {
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
        PlayerInBuilding?.Invoke(false);
    }

    public void LockDoors()
    {
        foreach (var door in _doors)
            door.Lock(true);
    }

    private void Start()
    {
        foreach (var enemy in _enemies)
            enemy.Initialize(this);
    }
}
