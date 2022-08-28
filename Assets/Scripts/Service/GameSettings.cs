using UnityEngine;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instanse { get; private set; } = null;

    [SerializeField] private Language language = Language.Russian;
    [SerializeField] private Color illuminationColor;

    [Header("Testing")]
    [SerializeField] private bool fog = true;

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

    [Header("Hearing Radiuses")]
    [SerializeField] private float hearingRadiusQuietly;
    [SerializeField] private float hearingRadiusLoudly;
    [SerializeField] private float hearingRadiusVeryLoudly;
    [SerializeField] private float hearingRadiusAfterOpeningDoor = 5f;
    [SerializeField] private float hearingRadiusDuringLoot;
    [SerializeField] private float hearingRadiusDuringEnemyScream;

    private Dictionary<ResourceType, Vector2> amountsResourcesFound;
    private Dictionary<LoudnessType, float> hearingRadiuses;

    public Language Language { get => language; set => language = value; }

    public Color IlluminationColor { get => illuminationColor; }

    public bool Fog { get => fog; }

    public float ChanceOfFindingMoney { get => chanceOfFindingMoney; }

    public float ChanceOfFinidngMainResource { get => chanceOfFinidngMainResource; }

    public float ChanceOfFindingEquipment { get => chanceOfFindingEquipment; }

    public float ChanceOfFindingMasterKeys { get => chanceOfFindingMasterKeys; }

    public float ChanceOfFindingTierIrons { get => chanceOfFindingTierIrons; }

    public float HearingRadiusAfterOpeningDoor { get => hearingRadiusAfterOpeningDoor; }

    public float HearingRadiusDuringLoot { get => hearingRadiusDuringLoot; }

    public float HearingRadiusDuringEnemyScream { get => hearingRadiusDuringEnemyScream; }

    public Vector2 GetAmountResourceFound(ResourceType type) => amountsResourcesFound[type];

    public float GetHearingRadius(LoudnessType type) => hearingRadiuses[type];

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

        hearingRadiuses = new Dictionary<LoudnessType, float>
        {
            [LoudnessType.Quietly] = hearingRadiusQuietly,
            [LoudnessType.Loudly] = hearingRadiusLoudly,
            [LoudnessType.VeryLoudly] = hearingRadiusVeryLoudly
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hearingRadiusQuietly);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadiusLoudly);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadiusVeryLoudly);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRadiusDuringLoot);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, hearingRadiusAfterOpeningDoor);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, hearingRadiusDuringEnemyScream);
    }
}
