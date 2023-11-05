using UnityEngine;
using System.Collections.Generic;
using System;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    [SerializeField] private ResourcesPanel _resourcesPanel;

    private Dictionary<ResourceType, int> _resources;

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
    }

    private void Start()
    {
        foreach (var resource in _resources)
            _resourcesPanel.SetResourceValue(resource.Key, resource.Value);
    }
}
