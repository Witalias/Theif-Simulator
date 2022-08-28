using UnityEngine;

public class Noisy : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).transform;
    }

    public void Noise(float radius)
    {
        var enemies = Physics.OverlapSphere(player.position, radius, GameStorage.Instanse.EnemyMask);

        foreach (var enemy in enemies)
        {
            var enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
                enemyAI.SetTargetPoint();
        }
    }
}
