using UnityEngine;
using System.Collections.Generic;

public class GameStorage : MonoBehaviour
{
    public static GameStorage Instanse { get; private set; } = null;

    [Header("Prefabs")]
    [SerializeField] private NewResourceAnimation newResourceAnimationPrefab;
    [SerializeField] private UIHotkey hotkeyPrefab;
    [SerializeField] private QuestionMark questionMarkPrefab;
    [SerializeField] private EnemyAI[] enemyPrefabs;

    [Header("Sprites")]
    [SerializeField] private Sprite _bootle;
    [SerializeField] private Sprite _sneakers;
    [SerializeField] private Sprite _money;

    [Header("Sounds")]
    [SerializeField] private Sound _bootleSound;
    [SerializeField] private Sound _moneySound;
    [SerializeField] private Sound _sneakersSound;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask enemyMask;

    private Dictionary<ResourceType, Sprite> resourceSprites;
    private Dictionary<ResourceType, Sound> resourceSounds;

    public GameObject NewResourceAnimatinPrefab { get => newResourceAnimationPrefab.gameObject; }

    public GameObject HotkeyPrefab { get => hotkeyPrefab.gameObject; }

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
            [ResourceType.Bootle] = _bootle,
            [ResourceType.Sneakers] = _sneakers,
            [ResourceType.Money] = _money,
        };

        resourceSounds = new Dictionary<ResourceType, Sound>
        {
            [ResourceType.Bootle] = _bootleSound,
            [ResourceType.Sneakers] = _sneakersSound,
            [ResourceType.Money] = _moneySound,
        };
    }
}
