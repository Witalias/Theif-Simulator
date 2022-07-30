using UnityEngine;
using System.Collections.Generic;

public class GameStorage : MonoBehaviour
{
    public static GameStorage Instanse { get; private set; } = null;

    [Header("Prefabs")]
    [SerializeField] private NewResourceAnimation newResourceAnimationPrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite appleIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite moneyIcon;
    [SerializeField] private Sprite fuelIcon;

    private Dictionary<ResourceType, Sprite> resourceSprites;

    public GameObject NewResourceAnimatinPrefab { get => newResourceAnimationPrefab.gameObject; }

    public Transform MainCanvas { get; private set; }

    public Sprite GetResourceSprite(ResourceType type) => resourceSprites[type];

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
            [ResourceType.Water] = waterIcon
        };
    }
}
