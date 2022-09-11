using UnityEngine;
using System.Collections.Generic;
using System;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    [SerializeField] private bool canIntentionallyNoise = false;

    [Header("Equipment")]
    [SerializeField] private EquipmentStats arms;
    [SerializeField] private EquipmentStats masterKey;
    [SerializeField] private EquipmentStats tierIron;
    [SerializeField] private EquipmentStats gadget;

    private ResourcesPanel resourcesPanel;

    private Dictionary<ResourceType, float> resources;
    private Dictionary<EquipmentType, EquipmentStats> equipment;
    private Dictionary<ResourceType, float> increasedResourceNumbers;

    public bool CanIntentionallyNoise 
    { 
        get => canIntentionallyNoise;
        set
        {
            canIntentionallyNoise = value;
            if (resourcesPanel != null)
                resourcesPanel.SetActiveNoiseHotkey(value);
        }
    }

    public bool VisibilityFromIntentionalNoise { get; set; } = true;

    public float IncreasedPlayerSpeedInPercents { get; private set; }

    public float IncreasedDoorNoiseInPercents { get; private set; }

    public float IncreasedHackingTime { get; set; }

    public float IncreasedHackingNoiseInPercents { get; set; }

    public float IncreasedVisibilityScaleInPercents { get; set; }

    public void SetExtraResourceNumber(ResourceType type, float value)
    {
        if (increasedResourceNumbers.ContainsKey(type))
            increasedResourceNumbers[type] = value;
    }

    public void SetIncreasedDoorNoise(float valueInPercents) => IncreasedDoorNoiseInPercents = valueInPercents;

    public void SetIncreasedPlayerSpeed(float valueInPercents)
    {
        IncreasedPlayerSpeedInPercents = valueInPercents;
        var player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        if (player != null)
            player.AddSpeed(valueInPercents);
    }

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

    public EquipmentStats GetEquipmentStats(EquipmentType type) => equipment[type];

    public float GetExtraResource(ResourceType type)
    {
        if (increasedResourceNumbers.ContainsKey(type))
            return increasedResourceNumbers[type];
        return 0f;
    }

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

        equipment = new Dictionary<EquipmentType, EquipmentStats>
        {
            [EquipmentType.Arms] = arms,
            [EquipmentType.Gadget] = gadget,
            [EquipmentType.MasterKey] = masterKey,
            [EquipmentType.TierIron] = tierIron
        };

        increasedResourceNumbers = new Dictionary<ResourceType, float>
        {
            [ResourceType.Food] = 0f,
            [ResourceType.Fuel] = 0f,
            [ResourceType.Money] = 0f,
            [ResourceType.Water] = 0f
        };
    }

    private void Start()
    {
        resourcesPanel = GameObject.FindGameObjectWithTag(Tags.ResourcesPanel.ToString()).GetComponent<ResourcesPanel>();

        foreach (var resource in resources)
            resourcesPanel.SetResourceValue(resource.Key, resource.Value);
    }
}
