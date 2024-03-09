using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;
using UnityEngine.Events;

public class Lootable : MonoBehaviour, IIdentifiable
{
    public static event Action<Action, Action> ShowHoldButton;
    public static event Action<ResourceType, int, int, int> PlayResourceAnimation;
    public static event Action<string, float, bool> ShowQuickMessage;
    public static event Action GOnLooted;

    [SerializeField] private bool _enabled = true;
    [SerializeField] private ContainedResource[] _containedResources;
    [SerializeField] private Sound sound;
    [SerializeField] private MovingFurnitureElements _movingFurnitureElements;
    [SerializeField] private TriggerZone _triggerZone;
    [SerializeField] private GameObject _appearHackingZoneTrigger;
    [SerializeField] private UnityEvent _onLooted;

    private readonly SavedData _savedData = new();
    private Action _afterLootingAction;
    private bool _isLooting;
    private bool _isEmpty;

    public UnityEvent OnLooted => _onLooted;
    public bool Enabled => _enabled;

    public int ID { get; set; }

    private void OnEnable()
    {
        _triggerZone.SubscribeOnEnter(OnPlayerEnter);
        _triggerZone.SubscribeOnExit(OnPlayerExit);
        _triggerZone.SubscribeOnStay(OnPlayerStay);
    }

    private void OnDisable()
    {
        _triggerZone.UnsubscribeOnEnter(OnPlayerEnter);
        _triggerZone.UnsubscribeOnExit(OnPlayerExit);
        _triggerZone.UnsubscribeOnStay(OnPlayerStay);
    }

    public SavedData Save()
    {
        _savedData.ID = ID;
        _savedData.IsEmpty = _isEmpty;
        return _savedData;
    }

    public void Load(SavedData data)
    {
        SetEmpty(data.IsEmpty);
    }

    public void Initialize(Action afterLootingAction)
    {
        _afterLootingAction = afterLootingAction;
    }

    public void SetActiveTriggerZone(bool value)
    {
        _triggerZone.gameObject.SetActive(value);
        _appearHackingZoneTrigger.SetActive(value);
    }

    private void OnPlayerEnter(MovementController player)
    {
        if (GameData.Instanse.Backpack.IsFull)
            ShowQuickMessage?.Invoke($"{Translation.GetFullBackpackName()}!", 1.0f, true);
    }

    private void OnPlayerStay(MovementController player)
    {
        if (_isEmpty || GameData.Instanse.Backpack.IsFull || player.Noticed || player.Busy)
            return;

        player.CanHide(false);

        if (!player.IsRunning && !_isLooting)
        {
            player.RotateTowards(_appearHackingZoneTrigger.transform.position);
            TakeResource(player);
        }

        _triggerZone.gameObject.SetActive(!_isLooting);
    }

    private void OnPlayerExit(MovementController player)
    {
        if (_isEmpty || GameData.Instanse.Backpack.IsFull || player.Noticed || player.Busy)
            return;

        player.CanHide(true);
    }

    public void SetEmpty(bool value)
    {
        if (!gameObject.activeSelf)
            return;

        _isEmpty = value;
        //_hackingArea.SetActive(!value);
        _appearHackingZoneTrigger.SetActive(!value);

        if (value == true)
            _movingFurnitureElements.MoveForward();
        else
            _movingFurnitureElements.MoveBack();
    }

    private void TakeResource(MovementController player)
    {
        _isLooting = true;
        SoundManager.Instanse.Play(sound);
        void ActionDone()
        {
            SetEmpty(true);
            _isLooting = false;
            player.CanHide(true);

            if (_containedResources.Length > 0)
            {
                var randomIndex = Randomizator.GetRandomIndexByChances(_containedResources.Select(item => item.DropChance).ToArray());
                var randomResource = _containedResources[randomIndex];
                var count = Randomizator.GetRandomIndexByChances(randomResource.CountsChances) + 1;

                TaskManager.Instance.ProcessTask(TaskType.TheftItems, count);
                TaskManager.Instance.ProcessTask(TaskType.TutorialRobHouse, 1);
                TaskManager.Instance.ProcessTask(TaskType.TheftCertainItems, randomResource.Type, count);

                if (randomResource.OnlyMinMaxRange)
                    count = (int)UnityEngine.Random.Range(randomResource.MinMaxCount.x, randomResource.MinMaxCount.y);
                GameData.Instanse.Backpack.AddResource(randomResource.Type, count);

                var xp = GameData.Instanse.TheftXPReward;
                GameData.Instanse.PlayerLevel.AddXP(xp);

                PlayResourceAnimation?.Invoke(randomResource.Type, count, xp, 0);
                SoundManager.Instanse.Play(GameData.Instanse.GetResourceSound(randomResource.Type));
            }
            _afterLootingAction?.Invoke();
            _onLooted?.Invoke();
            GOnLooted?.Invoke();
        }
        void ActionAbort()
        {
            _isLooting = false;
            _appearHackingZoneTrigger.SetActive(true);
            _triggerZone.gameObject.SetActive(true);
        }
        ShowHoldButton?.Invoke(ActionDone, ActionAbort);
    }

    [Serializable]
    public class SavedData
    {
        public int ID;
        public bool IsEmpty;
    }
}
