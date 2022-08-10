using UnityEngine;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instanse { get; private set; } = null;

    [SerializeField] private Language language = Language.Russian;
    [SerializeField] private Color illuminationColor;

    [Header("Testing")]
    [SerializeField] private bool fog = true;
    [SerializeField] private float fogYPosition = 12.49f;

    [Header("Number of resources found (from/to)")]
    [SerializeField] private Vector2 amountFuelFound;
    [SerializeField] private Vector2 amountFoodFound;
    [SerializeField] private Vector2 amountWaterFound;
    [SerializeField] private Vector2 amountMoneyFound;
    [SerializeField] private Vector2 amountMasterKeyFound;
    [SerializeField] private Vector2 amountTierIronFound;

    [Header("Finding chances")]
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingMoney;
    [SerializeField] [Range(0f, 100f)] private float chanceOfFinidngMainResource;
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingEquipment;

    [Header("Equipment chances")]
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingMasterKeys;
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingTierIrons;

    private Dictionary<ResourceType, Vector2> amountsResourcesFound;

    public Language Language { get => language; set => language = value; }

    public Color IlluminationColor { get => illuminationColor; }

    public bool Fog { get => fog; }

    public float FogYPosition { get => fogYPosition; }

    public float ChanceOfFindingMoney { get => chanceOfFindingMoney; }

    public float ChanceOfFinidngMainResource { get => chanceOfFinidngMainResource; }

    public float ChanceOfFindingEquipment { get => chanceOfFindingEquipment; }

    public float ChanceOfFindingMasterKeys { get => chanceOfFindingMasterKeys; }

    public float ChanceOfFindingTierIrons { get => chanceOfFindingTierIrons; }

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
            [ResourceType.Water] = amountWaterFound,
            [ResourceType.MasterKeys] = amountMasterKeyFound,
            [ResourceType.TierIrons] = amountTierIronFound
        };
    }
}
