using UnityEngine;
using System.Collections.Generic;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    private ResourcesPanel resourcesPanel;

    private Dictionary<ResourceType, float> resources;

    public void AddResource(ResourceType type, float value)
    {
        if (type == ResourceType.Food || type == ResourceType.Water)
            resources[type] = Mathf.Clamp(resources[type] + value, 0f, 100f);
        else
            resources[type] = Mathf.Clamp(resources[type] + value, 0f, Mathf.Infinity);
        resourcesPanel.SetResourceValue(type, resources[type]);
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
            [ResourceType.Fuel] = 10f
        };
    }

    private void Start()
    {
        resourcesPanel = GameObject.FindGameObjectWithTag(Tags.ResourcesPanel.ToString()).GetComponent<ResourcesPanel>();

        foreach (var resource in resources)
            resourcesPanel.SetResourceValue(resource.Key, resource.Value);
    }
}
