using UnityEngine;
using System.Collections.Generic;

public class GameStorage : MonoBehaviour
{
    public static GameStorage Instanse { get; private set; } = null;

    [Header("Prefabs")]
    [SerializeField] private NewResourceAnimation newResourceAnimationPrefab;
    [SerializeField] private UIHotkey hotkeyPrefab;
    [SerializeField] private ActionMenuButton actionMenuButtonPrefab;
    [SerializeField] private WaitingAndAction waitingAndActionPrefab;
    [SerializeField] private QuestionMark questionMarkPrefab;
    [SerializeField] private NoiseEffect noiseEffectPrefab;
    [SerializeField] private EnemyAI policemanPrefab;
    [SerializeField] private EnemyAI[] enemyPrefabs;

    [Header("Sprites")]
    [SerializeField] private Sprite appleIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite moneyIcon;
    [SerializeField] private Sprite fuelIcon;
    [SerializeField] private Sprite masterKeyIcon;
    [SerializeField] private Sprite tierIronIcon;
    [SerializeField] private Sprite padlock;
    [SerializeField] private Sprite signal;
    [SerializeField] private Sprite speed;
    [SerializeField] private Sprite suitcase;
    [SerializeField] private Sprite trap;
    [SerializeField] private Sprite policeBadge;
    [SerializeField] private Sprite eye;
    [SerializeField] private Sprite ear;
    [SerializeField] private Sprite boot;
    [SerializeField] private Sprite door;
    [SerializeField] private Sprite armsTime;
    [SerializeField] private Sprite masterKeyTime;
    [SerializeField] private Sprite tierIronTime;
    [SerializeField] private Sprite gadgetTime;
    [SerializeField] private Sprite armsEar;
    [SerializeField] private Sprite tierIronEar;
    [SerializeField] private Sprite equipmentTime;
    [SerializeField] private Sprite equipmentEar;
    [SerializeField] private Sprite applePlus;
    [SerializeField] private Sprite waterPlus;
    [SerializeField] private Sprite moneyPlus;
    [SerializeField] private Sprite fuelPlus;

    [Header("Sounds")]
    [SerializeField] private Sound foodSound;
    [SerializeField] private Sound waterSound;
    [SerializeField] private Sound moneySound;
    [SerializeField] private Sound fuelSound;
    [SerializeField] private Sound masterKeySound;
    [SerializeField] private Sound tierIronSound;
    [SerializeField] private Sound gadgetSound;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask enemyMask;

    private Dictionary<ResourceType, Sprite> resourceSprites;
    private Dictionary<ResourceType, Sound> resourceSounds;

    public GameObject NewResourceAnimatinPrefab { get => newResourceAnimationPrefab.gameObject; }

    public GameObject HotkeyPrefab { get => hotkeyPrefab.gameObject; }

    public GameObject ActionMenuButtonPrefab { get => actionMenuButtonPrefab.gameObject; }

    public GameObject WaitingAndActionPrefab { get => waitingAndActionPrefab.gameObject; }

    public GameObject QuestionMarkPrefab { get => questionMarkPrefab.gameObject; }

    public GameObject NoiseEffectPrefab { get => noiseEffectPrefab.gameObject; }

    public GameObject PolicemanPrefab { get => policemanPrefab.gameObject; }

    public Sprite Padlock { get => padlock; }

    public Sprite Signal { get => signal; }

    public Sprite Speed { get => speed; }

    public Sprite Suitcase { get => suitcase; }

    public Sprite Trap { get => trap; }

    public Sprite PoliceBadge { get => policeBadge; }

    public Sprite Eye { get => eye; }

    public Sprite Ear { get => ear; }

    public Sprite Boot { get => boot; }

    public Sprite Door { get => door; }

    public Sprite ArmsTime { get => armsTime; }

    public Sprite MasterKeyTime { get => masterKeyTime; }

    public Sprite TierIronTime { get => tierIronTime; }

    public Sprite GadgetTime { get => gadgetTime; }

    public Sprite ArmsEar { get => armsEar; }

    public Sprite TierIronEar { get => tierIronEar; }

    public Sprite EquipmentTime { get => equipmentTime; }

    public Sprite EquipmentEar { get => equipmentEar; }

    public Sprite ApplePlus { get => applePlus; }

    public Sprite WaterPlus { get => waterPlus; }

    public Sprite MoneyPlus { get => moneyPlus; }

    public Sprite FuelPlus { get => fuelPlus; }

    public Transform MainCanvas { get; private set; }

    public LayerMask EnemyMask { get => enemyMask; }

    public Sprite GetResourceSprite(ResourceType type) => resourceSprites[type];

    public Sound GetResourceSound(ResourceType type) => resourceSounds[type];

    public GameObject GetRandomEnemyPrefab() => enemyPrefabs[Random.Range(0, enemyPrefabs.Length)].gameObject;

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        var canvas = GameObject.FindGameObjectWithTag(Tags.MainCanvas.ToString());
        if (canvas == null) Debug.LogWarning("The main canvas was not found!");
        else MainCanvas = canvas.transform;

        resourceSprites = new Dictionary<ResourceType, Sprite>
        {
            [ResourceType.Food] = appleIcon,
            [ResourceType.Fuel] = fuelIcon,
            [ResourceType.Money] = moneyIcon,
            [ResourceType.Water] = waterIcon,
            [ResourceType.MasterKeys] = masterKeyIcon,
            [ResourceType.TierIrons] = tierIronIcon
        };

        resourceSounds = new Dictionary<ResourceType, Sound>
        {
            [ResourceType.Food] = foodSound,
            [ResourceType.Fuel] = fuelSound,
            [ResourceType.Gadgets] = gadgetSound,
            [ResourceType.MasterKeys] = masterKeySound,
            [ResourceType.Money] = moneySound,
            [ResourceType.TierIrons] = tierIronSound,
            [ResourceType.Water] = waterSound
        };
    }
}
