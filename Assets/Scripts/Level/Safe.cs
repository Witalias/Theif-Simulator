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
        _containedResources = GameData.Instanse.Backpack.GetResources();
        _lootable.SetEmpty(GameData.Instanse.Backpack.IsEmpty);

        if (!GameData.Instanse.Backpack.IsEmpty)
            Filled?.Invoke();
    }

    private void Theft()
    {
        if (_containedResources == null)
            return;

        SoundManager.Instanse.Play(Sound.GetLoudResource);
        foreach (var resource in _containedResources)
        {
            if (resource.Value <= 0)
                continue;
            GameData.Instanse.Backpack.AddResource(resource.Key, resource.Value);
            PlayResourceAnimationItem?.Invoke(resource.Key, resource.Value);
        }
        _containedResources = null;
    }
}
