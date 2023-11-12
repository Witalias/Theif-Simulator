using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private EnemyAI[] _enemies;
    [SerializeField] private Door[] _doors;

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
    }

    private void LockDoors()
    {
        foreach (var door in _doors)
            door.Lock(true);
    }
}
