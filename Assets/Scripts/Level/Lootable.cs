using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;

public class Lootable : MonoBehaviour, IIdentifiable
{
    public static event Action<Action, Action> GShowHoldButton;
    public static event Action<float, Action, Action, float> GShowTapAction;
    public static event Action<ResourceType, int, int, int> GPlayResourceAnimation;
    public static event Action<string, float, bool> GShowQuickMessage;
    public static event Action GOnLooted;

    [SerializeField] private bool _enabled = true;
    [SerializeField] private bool _regenerative;
    [SerializeField] private bool _valuable;
    [SerializeField] private HackingType _hackingType;
    [SerializeField, Tooltip("Если Hacking Type == Tap")] private float _tapTimer = 10.0f;
    [SerializeField] private AudioType sound;
    [SerializeField, Tooltip("Количество открытий: индекс + 1\nПри превышении лимита берётся последнее значение.")]
    private int[] _moneyValues;
    [SerializeField] private ContainedResource[] _containedResources;
    [Space(30)]
    [SerializeField] private MovingFurnitureElements _movingFurnitureElements;
    [SerializeField] private TriggerZone _hackingZone;
    [SerializeField] private TriggerZone _appearHackingZone;
    [SerializeField] private CustomTimer _refillTimer;
    [SerializeField] private GameObject _shineContainer;
    [SerializeField] private GameObject _normalShineParticle;
    [SerializeField] private GameObject _goldShineParticle;
    [SerializeField] private UnityEvent _onLooted;

    private readonly SavedData _savedData = new();
    private Queue<int> _moneyValuesQueue;
    private Action _afterLootingAction;
    private bool _isLooting;
    private bool _isEmpty;

    public UnityEvent OnLooted => _onLooted;
    public bool Enabled => _enabled;
    public bool Regenerative => _regenerative;
    public bool Valuable => _valuable;
    public HackingType HackingType => _hackingType;

    public int ID { get; set; }

    private void Awake()
    {
        _moneyValuesQueue = new(_moneyValues);
    }

    private void OnEnable()
    {
        _hackingZone.SubscribeOnEnter(OnHackingZoneEnter);
        _hackingZone.SubscribeOnExit(OnHackingZoneExit);
        _hackingZone.SubscribeOnStay(OnHackingZoneStay);
        _appearHackingZone.SubscribeOnEnter(OnAppearHackingZoneEnter);
        _appearHackingZone.SubscribeOnExit(OnAppearHackingZoneExit);
        _refillTimer.SubscribeOnComplete(OnRefillTimerComplete);
    }

    private void OnDisable()
    {
        _hackingZone.UnsubscribeOnEnter(OnHackingZoneEnter);
        _hackingZone.UnsubscribeOnExit(OnHackingZoneExit);
        _hackingZone.UnsubscribeOnStay(OnHackingZoneStay);
        _appearHackingZone.UnsubscribeOnEnter(OnAppearHackingZoneEnter);
        _appearHackingZone.UnsubscribeOnExit(OnAppearHackingZoneExit);
        _refillTimer.UnsubscribeOnComplete(OnRefillTimerComplete);
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

        if (_regenerative && data.IsEmpty)
            _refillTimer.Run();
    }

    public void Initialize(Action afterLootingAction)
    {
        _afterLootingAction = afterLootingAction;
    }

    public void SetActiveTriggerZone(bool value)
    {
        _hackingZone.gameObject.SetActive(value);
        _appearHackingZone.gameObject.SetActive(value);
    }

    public void SetActiveRefillTimer(bool value) => _refillTimer.gameObject.SetActive(value);

    public void SetActiveNormalShine(bool value) => _normalShineParticle.SetActive(value);

    public void SetActiveGoldShine(bool value) => _goldShineParticle.SetActive(value);

    public void SetActiveShineContainer(bool value) => _shineContainer.SetActive(value);

    public void SetEmpty(bool value)
    {
        if (!gameObject.activeSelf)
            return;

        _isEmpty = value;
        _appearHackingZone.GetComponent<AppearHackingZoneTrigger>().Enabled = !value;

        if (_valuable)
            SetActiveGoldShine(!value);
        else
            SetActiveNormalShine(!value);

        if (value == true)
            _movingFurnitureElements.MoveForward();
        else
            _movingFurnitureElements.MoveBack();
    }

    private void OnHackingZoneEnter(MovementController player)
    {
        if (GameData.Instanse.Backpack.IsFull)
            GShowQuickMessage?.Invoke($"{Translation.GetFullBackpackName()}!", 1.0f, true);
    }

    private void OnHackingZoneStay(MovementController player)
    {
        if (_isEmpty || GameData.Instanse.Backpack.IsFull || player.Noticed || player.Busy)
            return;

        player.CanHide(false);

        if (!player.IsRunning && !_isLooting)
        {
            player.RotateTowards(_appearHackingZone.transform.position);
            TakeResource(player);
        }

        _hackingZone.gameObject.SetActive(!_isLooting);
    }

    private void OnHackingZoneExit(MovementController player)
    {
        if (_isEmpty || GameData.Instanse.Backpack.IsFull || player.Noticed || player.Busy)
            return;

        player.CanHide(true);
    }

    private void OnAppearHackingZoneEnter(MovementController player)
    {
        if (_regenerative && _refillTimer.IsOn)
            _refillTimer.SetContentActive(true);
    }

    private void OnAppearHackingZoneExit(MovementController player)
    {
        if (_regenerative)
            _refillTimer.SetContentActive(false);
    }

    private void OnRefillTimerComplete()
    {
        SetEmpty(false);
        _refillTimer.SetContentActive(false);
    }

    private void TakeResource(MovementController player)
    {
        _isLooting = true;

        void ActionDone()
        {
            SetEmpty(true);
            _isLooting = false;
            player.CanHide(true);

            ResourceType resource = 0;
            var count = 0;
            var money = 0;
            var xp = _valuable ? GameData.Instanse.TheftValuableXPReward : GameData.Instanse.TheftXPReward;

            if (_containedResources.Length > 0)
            {
                var randomIndex = Randomizator.GetRandomIndexByChances(_containedResources.Select(item => item.DropChance).ToArray());
                var containedResource = _containedResources[randomIndex];
                resource = containedResource.Type;
                count = Randomizator.GetRandomIndexByChances(containedResource.CountsChances) + 1;

                TaskManager.Instance.ProcessTask(TaskType.TheftItems, count);
                TaskManager.Instance.ProcessTask(TaskType.TutorialRobHouse, 1);
                TaskManager.Instance.ProcessTask(TaskType.TheftCertainItems, containedResource.Type, count);

                if (containedResource.OnlyMinMaxRange)
                    count = (int)UnityEngine.Random.Range(containedResource.MinMaxCount.x, containedResource.MinMaxCount.y);
                GameData.Instanse.Backpack.AddResource(containedResource.Type, count);

                AudioManager.Instanse.Play(GameData.Instanse.GetResourceSound(containedResource.Type));

                if (_regenerative)
                {
                    _refillTimer.SetContentActive(true);
                    _refillTimer.Run();
                }
            }
            if (_moneyValuesQueue.Count > 0)
            {
                money = GetMoneyValue();
                GameData.Instanse.AddMoney(money);
                AudioManager.Instanse.Play(AudioType.GetMoney);
            }
            GameData.Instanse.PlayerLevel.AddXP(xp);
            GPlayResourceAnimation?.Invoke(resource, count, xp, money);

            _afterLootingAction?.Invoke();
            _onLooted?.Invoke();
            GOnLooted?.Invoke();
        }

        void ActionAbort()
        {
            _isLooting = false;
            _appearHackingZone.GetComponent<AppearHackingZoneTrigger>().Enabled = true;
            _hackingZone.gameObject.SetActive(true);
        }

        if (_hackingType is HackingType.Hold)
        {
            GShowHoldButton?.Invoke(ActionDone, ActionAbort);
            AudioManager.Instanse.Play(sound);
        }
        else
        {
            GShowTapAction?.Invoke(_tapTimer, ActionDone, ActionAbort, 0.0f);
            AudioManager.Instanse.Play(AudioType.MasterKey);
        }
    }

    private int GetMoneyValue()
    {
        if (_moneyValuesQueue.Count > 1)
            return _moneyValuesQueue.Dequeue();
        return _moneyValuesQueue.Peek();
    }

    [Serializable]
    public class SavedData
    {
        public int ID;
        public bool IsEmpty;
    }
}
