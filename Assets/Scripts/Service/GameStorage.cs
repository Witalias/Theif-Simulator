using UnityEngine;
using System.Collections.Generic;

public class GameStorage : MonoBehaviour
{
    public static GameStorage Instanse { get; private set; } = null;

    [Header("Prefabs")]
    [SerializeField] private NewResourceAnimation newResourceAnimationPrefab;
    [SerializeField] private UIHotkey hotkeyPrefab;
    [SerializeField] private ActionMenuButton actionMenuButtonPrefab;
    [SerializeField] private QuestionMark questionMarkPrefab;
    [SerializeField] private EnemyAI[] enemyPrefabs;

    [Header("Sprites")]
    [SerializeField] private Sprite appleIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite moneyIcon;
    [SerializeField] private Sprite fuelIcon;
    [SerializeField] private Sprite masterKeyIcon;
    [SerializeField] private Sprite tierIronIcon;
    [SerializeField] private Sprite padlock;

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

    public GameObject QuestionMarkPrefab { get => questionMarkPrefab.gameObject; }

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
