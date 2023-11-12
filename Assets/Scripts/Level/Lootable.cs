using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

[RequireComponent(typeof(MovingFurnitureElements))]
public class Lootable : MonoBehaviour
{
    public static event Action<Action, Action> ShowHoldButton;
    public static event Action<ResourceType, int, int> PlayResourceAnimation;

    [Tooltip("Counts Changes: индекс+1 - количество предметов")]
    [SerializeField] private ItemsDropChance[] _containedResources;
    [SerializeField] private Vector2 _minMaxXP;
    [SerializeField] private Sound sound;
    [SerializeField] private GameObject _hackingArea;
    [SerializeField] private GameObject _appearHackingZoneTrigger;

    private MovingFurnitureElements _movingFurnitureElements;
    private Camera _mainCamera;

    private bool _empty = false;
    private bool _isLooting = false;

    public void Loot(MovementController player)
    {
        if (_empty)
            return;

        if (!player.IsRunning && !_isLooting)
        {
            player.RotateTowards(_appearHackingZoneTrigger.transform.position);
            TakeResource(player.transform);
        }

        _hackingArea.SetActive(!_isLooting);
    }

    private void Awake()
    {
        _movingFurnitureElements = GetComponent<MovingFurnitureElements>();
        _mainCamera = Camera.main;
    }

    private void TakeResource(Transform player)
    {
        _isLooting = true;
        SoundManager.Instanse.Play(sound);
        void ActionDone()
        {
            _empty = true;
            _isLooting = false;
            _appearHackingZoneTrigger.SetActive(false);
            _movingFurnitureElements.Move();

            if (_containedResources.Length == 0)
                return;

            var randomIndex = Randomizator.GetRandomIndexByChances(_containedResources.Select(item => item.DropChance).ToArray());
            var randomResource = _containedResources[randomIndex];
            var count = Randomizator.GetRandomIndexByChances(randomResource.CountsChances) + 1;

            if (randomResource.OnlyMinMaxRange)
                count = (int)UnityEngine.Random.Range(randomResource.MinMaxCount.x, randomResource.MinMaxCount.y);
            Stats.Instanse.AddResource(randomResource.Type, count);

            var xp = Randomizator.GetRandomValue(_minMaxXP);
            Stats.Instanse.AddXP(xp);

            PlayResourceAnimation?.Invoke(randomResource.Type, count, xp);
            SoundManager.Instanse.Play(GameStorage.Instanse.GetResourceSound(randomResource.Type));
        }
        void ActionAbort()
        {
            _isLooting = false;
            _appearHackingZoneTrigger.SetActive(true);
        }
        ShowHoldButton?.Invoke(ActionDone, ActionAbort);
    }

    [Serializable]
    public class ItemsDropChance
    {
        public ResourceType Type;
        public float DropChance;
        public float[] CountsChances;
        public bool OnlyMinMaxRange;
        public Vector2 MinMaxCount;
    }
}
