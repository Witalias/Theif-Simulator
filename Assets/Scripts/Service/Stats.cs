using UnityEngine;
using System.Collections.Generic;
using System;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    [SerializeField] private int _initialLevel = 1;
    [SerializeField] private int _neededXP = 3;
    [SerializeField] private ResourcesPanel _resourcesPanel;
    [SerializeField] private XPBar _xpBar;

    private Dictionary<ResourceType, int> _resources;
    private int _xpAmount;

    public int Level { get; private set; }

    public void AddXP(int value)
    {
        _xpAmount += value;
        if (_xpAmount >= _neededXP)
        {
            _xpAmount -= _neededXP;
            NextLevel();
        }
        _xpBar.SetProgress(_xpAmount, _neededXP);
    }

    public void AddResource(ResourceType type, int value)
    {
        if (_resourcesPanel == null)
            return;

        _resources[type] = (int)Mathf.Clamp(_resources[type] + value, 0, Mathf.Infinity);
        _resourcesPanel.SetResourceValue(type, _resources[type]);
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
        AddXP(0);
    }

    private void Start()
    {
        foreach (var resource in _resources)
            _resourcesPanel.SetResourceValue(resource.Key, resource.Value);
    }

    private void NextLevel()
    {
        _xpBar.SetLevel(++Level);
        _neededXP += GameSettings.Instanse.StepXPRequirement;
    }
}
