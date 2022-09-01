using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Noisy : MonoBehaviour
{
    private const float noiseEffectsDelay = 3f;

    private List<EnemyAI> policemans;
    private Transform playerPoint;
    private Transform playerCenterPoint;
    private GameObject noiseEffect = null;
    private bool noiseEffectPlayed = true;

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

    public void AttractPolicemans()
    {
        foreach (var policeman in policemans)
            policeman.SetTargetPoint();
    }

    private void Start()
    {
        playerPoint = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).transform;
        policemans = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>().GetPolicemans();
        playerCenterPoint = playerPoint.GetComponent<MovementController>().CenterPoint;
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
