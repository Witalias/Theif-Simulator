using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class Safe : MonoBehaviour
{
    public static event Action<ResourceType, int> PlayResourceAnimationItem;
    public static event Action Filled;

    [SerializeField] private Lootable _lootable;

    private Dictionary<ResourceType, int> _containedResources;

    private void Awake()
    {
        _lootable.OnLooted.AddListener(Theft);
    }

    private void Start()
    {
        _lootable.SetEmpty(true);
    }

    private void OnEnable()
    {
        MovementController.PlayerCaught += Fill;
    }

    private void OnDisable()
    {
        MovementController.PlayerCaught -= Fill;
    }

    private void Fill()
    {
        _containedResources = Stats.Instanse.GetResources();
        _lootable.SetEmpty(Stats.Instanse.BackpackIsEmpty);

        if (!Stats.Instanse.BackpackIsEmpty)
            Filled?.Invoke();
    }

    private void Theft()
    {
        if (_containedResources == null)
            return;

        SoundManager.Instanse.Play(Sound.TierIron);
        foreach (var resource in _containedResources)
        {
            if (resource.Value <= 0)
                continue;
            Stats.Instanse.AddResource(resource.Key, resource.Value);
            PlayResourceAnimationItem?.Invoke(resource.Key, resource.Value);
        }
        _containedResources = null;
    }
}
