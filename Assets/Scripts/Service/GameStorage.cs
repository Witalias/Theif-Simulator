using UnityEngine;
using System.Collections.Generic;

public class GameStorage : MonoBehaviour
{
    public static GameStorage Instanse { get; private set; } = null;

    [Header("Prefabs")]
    [SerializeField] private NewResourceAnimation newResourceAnimationPrefab;
    [SerializeField] private EnemyAI[] enemyPrefabs;
    [SerializeField] private GameObject _playerBoxPrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite _bootle;
    [SerializeField] private Sprite _sneakers;
    [SerializeField] private Sprite _money;
    [SerializeField] private Sprite _xp;

    [Header("Sounds")]
    [SerializeField] private Sound _bootleSound;
    [SerializeField] private Sound _moneySound;
    [SerializeField] private Sound _sneakersSound;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask enemyMask;

    private Dictionary<ResourceType, Sprite> resourceSprites;
    private Dictionary<ResourceType, Sound> resourceSounds;

    public GameObject NewResourceAnimatinPrefab => newResourceAnimationPrefab.gameObject;

    public GameObject PlayerBoxPrefab => _playerBoxPrefab;

    public Transform MainCanvas { get; private set; }

    public Sprite Star => _xp;

    public LayerMask EnemyMask => enemyMask;

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
        };

        resourceSounds = new Dictionary<ResourceType, Sound>
        {
            [ResourceType.Bootle] = _bootleSound,
            [ResourceType.Sneakers] = _sneakersSound,
        };
    }
}
