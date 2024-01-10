using UnityEngine;
using System.Collections.Generic;

public class GameStorage : MonoBehaviour
{
    public static GameStorage Instanse { get; private set; } = null;

    [System.Serializable]
    private class ResourceData
    {
        public ResourceType Type;
        public Sprite Sprite;
        public Sound Sound;
        public int Price;
    }

    [SerializeField] private ResourceData[] _resourceData;

    [Header("Prefabs")]
    [SerializeField] private NewResourceAnimation newResourceAnimationPrefab;
    [SerializeField] private EnemyAI[] enemyPrefabs;

    [Header("Sprites")]
    [SerializeField] private Sprite _money;
    [SerializeField] private Sprite _xp;

    [Header("Sounds")]
    [SerializeField] private Sound _moneySound;

    [Header("Points")]
    [SerializeField] private Transform _prisonSpawnPoint;
    [SerializeField] private Transform _initialPlayerSpawnPoint;

    private Dictionary<ResourceType, ResourceData> _resources = new();

    public GameObject NewResourceAnimatinPrefab => newResourceAnimationPrefab.gameObject;

    public Transform MainCanvas { get; private set; }

    public Sprite Star => _xp;

    public Sprite Money => _money;

    public Transform PrisonSpawnPoint => _prisonSpawnPoint;

    public Transform InitialPlayerSpawnPoint => _initialPlayerSpawnPoint;

    public Sprite GetResourceSprite(ResourceType type) => _resources[type].Sprite;

    public Sound GetResourceSound(ResourceType type) => _resources[type].Sound;

    public int GetResourcePrice(ResourceType type) => _resources[type].Price;

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

        foreach (var resource in _resourceData)
            _resources.Add(resource.Type, resource);
    }
}
