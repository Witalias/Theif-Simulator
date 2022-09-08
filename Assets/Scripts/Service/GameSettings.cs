using UnityEngine;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instanse { get; private set; } = null;

    [SerializeField] private Language language = Language.Russian;
    [SerializeField] private Color illuminationColor;

    [Header("Number of resources found (from/to)")]
    [SerializeField] private Vector2 amountFuelFound;
    [SerializeField] private Vector2 amountFoodFound;
    [SerializeField] private Vector2 amountWaterFound;
    [SerializeField] private Vector2 amountMoneyFound;
    [SerializeField] private Vector2 amountMasterKeyFound;
    [SerializeField] private Vector2 amountTierIronFound;
    [SerializeField] private Vector2 amountGadgetFound;

    [Header("Finding chances")]
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingMoney;
    [SerializeField] [Range(0f, 100f)] private float chanceOfFinidngMainResource;
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingEquipment;

    [Header("Equipment chances")]
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingMasterKeys;
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingTierIrons;
    [SerializeField] [Range(0f, 100f)] private float chanceOfFindingGadgets;

    [Header("Hearing Radiuses")]
    [SerializeField] private float hearingRadiusQuietly;
    [SerializeField] private float hearingRadiusLoudly;
    [SerializeField] private float hearingRadiusVeryLoudly;
    [SerializeField] private float hearingRadiusAfterOpeningDoor = 5f;
    [SerializeField] private float hearingRadiusDuringLoot;
    [SerializeField] private float hearingRadiusDuringEnemyScream;
    [SerializeField] private float hearingRadiusAfterPlayerNoise;

    [Header("Visibility Values")]
    [SerializeField] [Range(0f, 5f)] private float visibilityValueSuspicion;
    [SerializeField] [Range(0f, 5f)] private float visibilityValueDetection;
    [SerializeField] private float increaseInResidentSpeed = 2f;
    [SerializeField] private float increaseInResidentViewAngle = 30f;
    [SerializeField] private float increaseInResidentViewDistance = 10f;
    [SerializeField] private float increaseInHackingTimeWithArms = 10f;
    [SerializeField] private float increaseInHackingTimeWithMasterKey = 6f;
    [SerializeField] private float increaseInHackingTimeWithTierIron = 3f;
    [SerializeField] private float increaseInHackingTimeWithGadget = 0f;
    [SerializeField] private float hearingRadiusMultiplier = 1.25f;

    [Header("Visibility Bolleans")]
    [SerializeField] private bool noResidentsReactionOnIntentionalNoise = false;
    [SerializeField] private bool doubleLocks = false;
    [SerializeField] private bool increasedHearingRadius = false;

    private Dictionary<ResourceType, Vector2> amountsResourcesFound;
    private Dictionary<LoudnessType, float> hearingRadiuses;
    private Dictionary<EquipmentType, float> increasesInHackingTimes;
    private float initialHearingRadiusAfterOpeningDoor;

    public Language Language { get => language; set => language = value; }

    public Color IlluminationColor { get => illuminationColor; }

    public float ChanceOfFindingMoney { get => chanceOfFindingMoney; }

    public float ChanceOfFinidngMainResource { get => chanceOfFinidngMainResource; }

    public float ChanceOfFindingEquipment { get => chanceOfFindingEquipment; }

    public float ChanceOfFindingMasterKeys { get => chanceOfFindingMasterKeys; }

    public float ChanceOfFindingTierIrons { get => chanceOfFindingTierIrons; }

    public float ChanceOfFindingGadgets { get => chanceOfFindingGadgets; }

    public float HearingRadiusAfterOpeningDoor { get => hearingRadiusAfterOpeningDoor; private set => hearingRadiusAfterOpeningDoor = value; }

    public float HearingRadiusDuringLoot { get => hearingRadiusDuringLoot; }

    public float HearingRadiusDuringEnemyScream { get => hearingRadiusDuringEnemyScream; }

    public float HearingRadiusAfterPlayerNoise { get => hearingRadiusAfterPlayerNoise; }

    public float VisibilityValueSuspicion { get => visibilityValueSuspicion; }

    public float VisibilityValueDetection { get => visibilityValueDetection; }

    public float IncreaseInResidentSpeed { get => increaseInResidentSpeed; }

    public float IncreaseInResidentViewAngle { get => increaseInResidentViewAngle; }

    public float IncreaseInResidentViewDistance { get => increaseInResidentViewDistance; }

    public float HearingRadiusMultiplier { get => hearingRadiusMultiplier; }

    public bool NoResidentsReactionOnIntentionalNoise { get => noResidentsReactionOnIntentionalNoise; set => noResidentsReactionOnIntentionalNoise = value; }

    public bool DoubleLocks { get => doubleLocks; set => doubleLocks = value; }

    public bool IncreasedHearingRadius { get => increasedHearingRadius; set => increasedHearingRadius = value; }

    public Vector2 GetAmountResourceFound(ResourceType type) => amountsResourcesFound[type];

    public float GetHearingRadius(LoudnessType type) => hearingRadiuses[type];

    public float GetIncreaseInHackingTime(EquipmentType type) => increasesInHackingTimes[type];

    public void AddHearingRadiusAfterOpeningDoor(float valueInPercents) => hearingRadiusAfterOpeningDoor += initialHearingRadiusAfterOpeningDoor * valueInPercents / 100f;

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        initialHearingRadiusAfterOpeningDoor = hearingRadiusAfterOpeningDoor;

        amountsResourcesFound = new Dictionary<ResourceType, Vector2>
        {
            [ResourceType.Food] = amountFoodFound,
            [ResourceType.Fuel] = amountFuelFound,
            [ResourceType.Money] = amountMoneyFound,
            [ResourceType.Water] = amountWaterFound,
            [ResourceType.MasterKeys] = amountMasterKeyFound,
            [ResourceType.TierIrons] = amountTierIronFound,
            [ResourceType.Gadgets] = amountGadgetFound
        };

        hearingRadiuses = new Dictionary<LoudnessType, float>
        {
            [LoudnessType.Quietly] = hearingRadiusQuietly,
            [LoudnessType.Loudly] = hearingRadiusLoudly,
            [LoudnessType.VeryLoudly] = hearingRadiusVeryLoudly
        };

        increasesInHackingTimes = new Dictionary<EquipmentType, float>
        {
            [EquipmentType.Arms] = increaseInHackingTimeWithArms,
            [EquipmentType.MasterKey] = increaseInHackingTimeWithMasterKey,
            [EquipmentType.TierIron] = increaseInHackingTimeWithTierIron,
            [EquipmentType.Gadget] = increaseInHackingTimeWithGadget
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

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, hearingRadiusAfterPlayerNoise);
    }
}
