using UnityEngine;
using System.Collections.Generic;
using System;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    [Header("Equipment")]
    [SerializeField] private EquipmentStats arms;
    [SerializeField] private EquipmentStats masterKey;
    [SerializeField] private EquipmentStats tierIron;
    [SerializeField] private EquipmentStats gadget;

    private ResourcesPanel resourcesPanel;

    private Dictionary<ResourceType, float> resources;

    public void AddResource(ResourceType type, float value)
    {
        if (resourcesPanel == null)
            return;

        if (type == ResourceType.Food || type == ResourceType.Water)
            resources[type] = Mathf.Clamp(resources[type] + value, 0f, 100f);
        else
            resources[type] = Mathf.Clamp(resources[type] + value, 0f, Mathf.Infinity);
        resourcesPanel.SetResourceValue(type, resources[type]);
    }

    public void AddResource(EquipmentType type, float value)
    {
        if (type == EquipmentType.Arms)
            return;

        AddResource(GameSettings.Instanse.GetResourceTypeByEquipmentType(type), value);
    }

    public float GetResource(ResourceType type) => resources[type];

    public float GetResource(EquipmentType type) => GetResource(GameSettings.Instanse.GetResourceTypeByEquipmentType(type));

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        resources = new Dictionary<ResourceType, float>
        {
            [ResourceType.Food] = 50f,
            [ResourceType.Water] = 50f,
            [ResourceType.Money] = 0f,
            [ResourceType.Fuel] = 10f,
            [ResourceType.MasterKeys] = 1f,
            [ResourceType.TierIrons] = 1f,
            [ResourceType.Gadgets] = 1f
        };
    }

    private void Start()
    {
        resourcesPanel = GameObject.FindGameObjectWithTag(Tags.ResourcesPanel.ToString()).GetComponent<ResourcesPanel>();

        foreach (var resource in resources)
            resourcesPanel.SetResourceValue(resource.Key, resource.Value);
    }
}
