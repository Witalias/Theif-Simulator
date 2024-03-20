using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

public class BackpackController
{
    public static event Action<string, float, bool> ShowQuickMessage;

    private Dictionary<ResourceType, int> _resources = new();
    private ResourcesPanel _resourcesPanel;

    public int Capacity { get; private set; }
    public int Fullness { get; private set; }
    public bool IsEmpty => Fullness <= 0;
    public bool IsFull => Fullness >= Capacity;

    public BackpackController(ResourcesPanel resourcesPanel)
    {
        _resourcesPanel = resourcesPanel;
        _resourcesPanel.Initialize();

        if (SaveLoad.HasResourcesSave)
        {
            _resources = SaveLoad.LoadResources();
            Fullness = _resources.Values.Sum();
        }
        else
        {
            foreach (var resourceType in Enum.GetValues(typeof(ResourceType)))
                _resources.Add((ResourceType)resourceType, 0);
        }

        foreach (var resource in _resources)
        {
            _resourcesPanel.SetResourceValue(resource.Key, resource.Value);
            _resourcesPanel.SetActiveCounter(resource.Key, resource.Value > 0);
        }
        UpdateCapacity();
    }

    public Dictionary<ResourceType, int> GetResources() => new(_resources);

    public int GetResourceCount(ResourceType type) => _resources[type];

    public void AddFullness(int value) => Fullness = Mathf.Clamp(Fullness + value, 0, Capacity);

    public void SetCapacity(int value)
    {
        Capacity = value;
        UpdateCapacity();
    }

    public void AddResource(ResourceType type, int value)
    {
        if (_resourcesPanel == null || IsFull)
            return;

        if (Fullness + value > Capacity)
            value = Capacity - Fullness;
        AddFullness(value);

        _resources[type] = (int)Mathf.Clamp(_resources[type] + value, 0, Mathf.Infinity);
        _resourcesPanel.SetResourceValue(type, _resources[type]);
        _resourcesPanel.SetActiveCounter(type, _resources[type] > 0);
        UpdateCapacity();

        if (YandexGame.savesData.TutorialRobHouseDone)
            SaveLoad.SaveResources(_resources);
    }

    public void ClearBackpack()
    {
        foreach (var resource in Enum.GetValues(typeof(ResourceType)))
            ClearResource((ResourceType)resource, false);
        _resourcesPanel.ClearResources();
        SaveLoad.SaveResources(_resources);
    }

    public void ClearResource(ResourceType type, bool save = true)
    {
        AddFullness(-_resources[type]);
        _resources[type] = 0;
        _resourcesPanel.SetActiveCounter(type, false);
        UpdateCapacity();

        if (save)
            SaveLoad.SaveResources(_resources);
    }

    public void SetMoney(int value) => _resourcesPanel.SetMoney(value);

    private void UpdateCapacity()
    {
        _resourcesPanel.SetBackpackCapacity(Fullness, Capacity);
        if (IsFull)
            ShowQuickMessage?.Invoke($"{Translation.GetFullBackpackName()}!", 1.0f, true);
    }
}
