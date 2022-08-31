using UnityEngine;
using System.Collections;

public class Noisy : MonoBehaviour
{
    private const float noiseEffectsDelay = 3f;

    private Transform playerPoint;
    private Transform playerCenterPoint;
    private GameObject noiseEffect = null;

    private bool noiseEffectPlayed = true;

    private void Start()
    {
        playerPoint = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).transform;
        playerCenterPoint = playerPoint.GetComponent<MovementController>().CenterPoint;
    }

    public void Noise(float radius, bool intentional = false)
    {
        if (noiseEffect == null && noiseEffectPlayed)
        {
            noiseEffectPlayed = false;
            CreateNoiseEffect(radius);
            StartCoroutine(StartDelay());
        }

        if (GameSettings.Instanse.NoResidentsReactionOnIntentionalNoise && intentional)
            return;

        var enemies = Physics.OverlapSphere(playerPoint.position, radius, GameStorage.Instanse.EnemyMask);
        foreach (var enemy in enemies)
        {
            var enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
                enemyAI.SetTargetPoint();
        }
    }

    private void CreateNoiseEffect(float radius)
    {
        noiseEffect = Instantiate(GameStorage.Instanse.NoiseEffectPrefab, playerCenterPoint.position, Quaternion.Euler(90, 0, 0));
        noiseEffect.GetComponent<NoiseEffect>().Play(radius);
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(noiseEffectsDelay);
        noiseEffectPlayed = true;
    }
}
