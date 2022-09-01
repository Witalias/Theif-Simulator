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

    [Header("Layer Masks")]
    [SerializeField] private LayerMask enemyMask;

    private Dictionary<ResourceType, Sprite> resourceSprites;

    public GameObject NewResourceAnimatinPrefab { get => newResourceAnimationPrefab.gameObject; }

    public GameObject HotkeyPrefab { get => hotkeyPrefab.gameObject; }

    public GameObject ActionMenuButtonPrefab { get => actionMenuButtonPrefab.gameObject; }

    public GameObject WaitingAndActionPrefab { get => waitingAndActionPrefab.gameObject; }

    public GameObject QuestionMarkPrefab { get => questionMarkPrefab.gameObject; }

    public GameObject NoiseEffectPrefab { get => noiseEffectPrefab.gameObject; }

    public GameObject PolicemanPrefab { get => policemanPrefab.gameObject; }

    public Transform MainCanvas { get; private set; }

    public LayerMask EnemyMask { get => enemyMask; }

    public Sprite GetResourceSprite(ResourceType type) => resourceSprites[type];

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
    }
}
