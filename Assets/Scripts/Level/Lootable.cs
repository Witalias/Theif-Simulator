using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

[RequireComponent(typeof(MovingFurnitureElements))]
public class Lootable : MonoBehaviour
{
    [Serializable]
    private class ItemsDropChance
    {
        public ResourceType Type;
        public float DropChance;
        public float[] CountsChances;
        public bool OnlyMinMaxRange;
        public Vector2 MinMaxCount;
    }

    private const string FULL_BACKPACK_TEXT = "FULL BACKPACK!";

    public static event Action<Action, Action> ShowHoldButton;
    public static event Action<ResourceType, int, int, int> PlayResourceAnimation;
    public static event Action<string, float> ShowQuickMessage;

    [Tooltip("Counts Changes: индекс+1 - количество предметов")]
    [SerializeField] private ItemsDropChance[] _containedResources;
    [SerializeField] private Vector2 _minMaxXP;
    [SerializeField] private Sound sound;
    [SerializeField] private GameObject _hackingArea;
    [SerializeField] private GameObject _appearHackingZoneTrigger;

    private MovingFurnitureElements _movingFurnitureElements;
    private Action _afterLootingAction;
    private bool _empty = false;
    private bool _isLooting = false;

    public void Initialize(Action afterLootingAction)
    {
        _afterLootingAction = afterLootingAction;
    }

    public void OnPlayerEnter()
    {
        if (Stats.Instanse.BackpackIsFull)
            ShowQuickMessage?.Invoke(FULL_BACKPACK_TEXT, 1.0f);
    }

    public void OnPlayerStay(MovementController player)
    {
        if (_empty || Stats.Instanse.BackpackIsFull || player.Noticed || player.Busy)
            return;

        player.CanHide(false);

        if (!player.IsRunning && !_isLooting)
        {
            player.RotateTowards(_appearHackingZoneTrigger.transform.position);
            TakeResource(player);
        }

        _hackingArea.SetActive(!_isLooting);
    }

    public void OnPlayerExit(MovementController player)
    {
        if (_empty || Stats.Instanse.BackpackIsFull || player.Noticed || player.Busy)
            return;

        player.CanHide(true);
    }

    public void Fill()
    {
        if (!gameObject.activeSelf)
            return;

        _empty = false;
        _appearHackingZoneTrigger.SetActive(true);
        _movingFurnitureElements.MoveBack();
    }

    private void Awake()
    {
        _movingFurnitureElements = GetComponent<MovingFurnitureElements>();
    }

    private void TakeResource(MovementController player)
    {
        _isLooting = true;
        SoundManager.Instanse.Play(sound);
        void ActionDone()
        {
            _empty = true;
            _isLooting = false;
            _appearHackingZoneTrigger.SetActive(false);
            _movingFurnitureElements.MoveForward();
            _afterLootingAction?.Invoke();
            player.CanHide(true);

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

            PlayResourceAnimation?.Invoke(randomResource.Type, count, xp, 0);
            SoundManager.Instanse.Play(GameStorage.Instanse.GetResourceSound(randomResource.Type));

            TaskManager.Instance.ProcessTask(TaskType.TheftItems, count);
            TaskManager.Instance.ProcessTask(TaskType.TheftCertainItems, randomResource.Type, count);
        }
        void ActionAbort()
        {
            _isLooting = false;
            _appearHackingZoneTrigger.SetActive(true);
            _hackingArea.SetActive(true);
        }
        ShowHoldButton?.Invoke(ActionDone, ActionAbort);
    }
}
