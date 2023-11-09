using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private EnemyAI[] _enemies;

    public void CalmEnemies()
    {
        foreach (var enemy in _enemies)
            enemy.Calm();
    }
}
