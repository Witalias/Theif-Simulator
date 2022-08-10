using UnityEngine;
using System.Collections.Generic;
using System;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    [SerializeField] private EquipmentStats arms;
    [SerializeField] private EquipmentStats masterKey;
    [SerializeField] private EquipmentStats tierIron;
    [SerializeField] private EquipmentStats gadget;

    private ResourcesPanel resourcesPanel;

    private Dictionary<ResourceType, float> resources;
    private Dictionary<EquipmentType, EquipmentStats> equipment;

    public void AddResource(ResourceType type, float value)
    {
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

        AddResource(GetResourceTypeByEquipmentType(type), value);
    }

    public float GetResource(ResourceType type) => resources[type];

    public float GetResource(EquipmentType type) => GetResource(GetResourceTypeByEquipmentType(type));

    public EquipmentStats GetEquipmentStats(EquipmentType type) => equipment[type];

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
            [ResourceType.MasterKeys] = 0f,
            [ResourceType.TierIrons] = 0f
        };

        equipment = new Dictionary<EquipmentType, EquipmentStats>
        {
            [EquipmentType.Arms] = arms,
            [EquipmentType.Gadget] = gadget,
            [EquipmentType.MasterKey] = masterKey,
            [EquipmentType.TierIron] = tierIron
        };
    }

    private void Start()
    {
        resourcesPanel = GameObject.FindGameObjectWithTag(Tags.ResourcesPanel.ToString()).GetComponent<ResourcesPanel>();

        foreach (var resource in resources)
            resourcesPanel.SetResourceValue(resource.Key, resource.Value);
    }

    private ResourceType GetResourceTypeByEquipmentType(EquipmentType type)
    {
        return type switch
        {
            EquipmentType.MasterKey => ResourceType.MasterKeys,
            EquipmentType.TierIron => ResourceType.TierIrons,
            _ => throw new Exception($"The resource {type} does not exist"),
        };
    }
}
