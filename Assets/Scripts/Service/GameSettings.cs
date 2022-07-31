using UnityEngine;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instanse { get; private set; } = null;

    [SerializeField] private Color illuminationColor;

    [Header("Testing")]
    [SerializeField] private bool fog = true;
    [SerializeField] private float fogYPosition = 12.49f;

    [Header("Number of resources found (from/to)")]
    [SerializeField] private Vector2 amountFuelFound;
    [SerializeField] private Vector2 amountFoodFound;
    [SerializeField] private Vector2 amountWaterFound;
    [SerializeField] private Vector2 amountMoneyFound;

    [Header("Chances")]
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingMoney;
    [SerializeField] [Range(0f, 100f)] private float chanceOfFinidngMainResource;

    private Dictionary<ResourceType, Vector2> amountsResourcesFound;

    public bool Fog { get => fog; }

    public float FogYPosition { get => fogYPosition; }

    public Color IlluminationColor { get => illuminationColor; }

    public Vector2 AmountFuelFound { get => amountFuelFound; }

    public Vector2 AmountFoodFound { get => amountFoodFound; }

    public Vector2 AmountWaterFound { get => amountWaterFound; }

    public float ChanceOfFindingMoney { get => chanceOfFindingMoney; }

    public float ChanceOfFinidngMainResource { get => chanceOfFinidngMainResource; }


    public Vector2 GetAmountResourceFound(ResourceType type) => amountsResourcesFound[type];

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        amountsResourcesFound = new Dictionary<ResourceType, Vector2>
        {
            [ResourceType.Food] = amountFoodFound,
            [ResourceType.Fuel] = amountFuelFound,
            [ResourceType.Money] = amountMoneyFound,
            [ResourceType.Water] = amountWaterFound
        };
    }
}
