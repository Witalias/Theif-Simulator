using UnityEngine;
using System.Collections.Generic;
using System;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    [SerializeField] private int _initialLevel = 1;
    [SerializeField] private int _backpackCapacity = 2;
    [SerializeField] private int _neededXP = 3;
    [SerializeField] private ResourcesPanel _resourcesPanel;
    [SerializeField] private XPBar _xpBar;
    [SerializeField] private Transform _prisonSpawnPoint;

    private Dictionary<ResourceType, int> _resources;
    private int _xpAmount;
    private int _backpackFullness;

    public int Level { get; private set; }

    public bool BackpackIsFull => _backpackFullness >= _backpackCapacity;

    public Transform PrisonSpawnPoint => _prisonSpawnPoint;

    public void AddXP(int value)
    {
        _xpAmount += value;
        if (_xpAmount >= _neededXP)
        {
            _xpAmount -= _neededXP;
            NextLevel();
        }
        _xpBar.SetProgress(_xpAmount, _neededXP);

        if (Level >= GameSettings.Instanse.MaxLevel)
            _xpBar.SetMaxLevelState();
    }

    public void AddResource(ResourceType type, int value)
    {
        if (_resourcesPanel == null || BackpackIsFull)
            return;

        if (type != ResourceType.Money)
        {
            if (_backpackFullness + value > _backpackCapacity)
                value -= _backpackCapacity - _backpackFullness;
            _backpackFullness += value;
        }

        _resources[type] = (int)Mathf.Clamp(_resources[type] + value, 0, Mathf.Infinity);
        _resourcesPanel.SetResourceValue(type, _resources[type]);
        _resourcesPanel.SetActiveCounter(type, _resources[type] > 0);
        _resourcesPanel.SetBackpackCapacity(_backpackFullness, _backpackCapacity);
    }

    public float GetResource(ResourceType type) => _resources[type];

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        _resources = new Dictionary<ResourceType, int>
        {
            [ResourceType.Bootle] = 0,
            [ResourceType.Sneakers] = 0,
            [ResourceType.Money] = 0,
        };

        Level = _initialLevel;
        _xpBar.SetLevel(_initialLevel);
        _resourcesPanel.SetBackpackCapacity(_backpackFullness, _backpackCapacity);
    }

    private void Start()
    {
        foreach (var resource in _resources)
            _resourcesPanel.SetResourceValue(resource.Key, resource.Value);
        AddXP(0);
    }

    private void NextLevel()
    {
        _xpBar.SetLevel(++Level);
        _neededXP += GameSettings.Instanse.StepXPRequirement;
    }
}
