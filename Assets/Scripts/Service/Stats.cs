using UnityEngine;
using System.Collections.Generic;
using System;
using YG;
using System.Linq;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    public static event Action<int> NewLevelReached;
    public static event Action<string, float> ShowQuickMessage;

    [SerializeField] private int _money = 0;
    [SerializeField] private float _playerMovingSpeed;
    [SerializeField] private float _tapBonusTimePercents = 5.0f;
    [SerializeField] private float _fillSpeedForHoldButton = 0.3f;
    [SerializeField] private int _initialLevel = 1;
    [SerializeField] private int _backpackCapacity = 2;
    [SerializeField] private int _neededXP = 3;
    [SerializeField] private ResourcesPanel _resourcesPanel;
    [SerializeField] private XPBar _xpBar;

    private Dictionary<ResourceType, int> _resources = new();
    private int _xpAmount;
    private int _backpackFullness;
    private int _soldItemsCount;

    public int Level { get; private set; }
    public int Money => _money;
    public float PlayerMovingSpeed { get => _playerMovingSpeed; set => _playerMovingSpeed = value; }
    public float TapBonusTimePercents { get => _tapBonusTimePercents; set => _tapBonusTimePercents = value; }
    public float FillSpeedForHoldButton { get => _fillSpeedForHoldButton; set => _fillSpeedForHoldButton = value; }
    public int BackpackCapacity { get => _backpackCapacity; set => _backpackCapacity = value; }
    public bool BackpackIsEmpty => _backpackFullness <= 0;
    public bool BackpackIsFull => _backpackFullness >= _backpackCapacity;

    public Dictionary<ResourceType, int> GetResources() => new(_resources);

    public void AddSoldItemsCount(int value)
    {
        _soldItemsCount += value;
        SaveLoad.SaveSoldItemsCount(_soldItemsCount);
        YandexGame.NewLeaderboardScores(GameStorage.Instanse.LeaderboardName, _soldItemsCount);
    }

    public void AddXP(int value)
    {
        _xpAmount += value;
        if (_xpAmount >= _neededXP)
        {
            _xpAmount -= _neededXP;
            NextLevel();
        }
        UpdateXPBar();
        SaveLoad.SaveLevel(Level, _xpAmount, _neededXP);
    }

    public void AddResource(ResourceType type, int value)
    {
        if (_resourcesPanel == null || BackpackIsFull)
            return;

        if (_backpackFullness + value > _backpackCapacity)
            value -= _backpackCapacity - _backpackFullness;
        _backpackFullness += value;

        _resources[type] = (int)Mathf.Clamp(_resources[type] + value, 0, Mathf.Infinity);
        _resourcesPanel.SetResourceValue(type, _resources[type]);
        _resourcesPanel.SetActiveCounter(type, _resources[type] > 0);
        UpdateCapacity();
        SaveLoad.SaveResources(_resources);
    }

    public void AddMoney(int value, bool assignTask = true)
    {
        _money = Mathf.Clamp(_money + value, 0, int.MaxValue);
        _resourcesPanel.SetMoney(_money);

        if (assignTask && value > 0)
            TaskManager.Instance.ProcessTask(TaskType.EarnMoney, value);

        SaveLoad.SaveMoney(_money);
    }

    public int GetResourceCount(ResourceType type) => _resources[type];

    public void ClearBackpack()
    {
        foreach (var resource in Enum.GetValues(typeof(ResourceType)))
            ClearResource((ResourceType)resource, false);
        _resourcesPanel.ClearResources();
        SaveLoad.SaveResources(_resources);
    }

    public void ClearResource(ResourceType type, bool save = true)
    {
        _backpackFullness -= _resources[type];
        _resources[type] = 0;
        _resourcesPanel.SetActiveCounter(type, false);
        UpdateCapacity();

        if (save)
            SaveLoad.SaveResources(_resources);
    }

    public float GetUpgradableValue(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.MoveSpeed => Stats.Instanse.PlayerMovingSpeed,
            UpgradeType.HackingSpeed => Stats.Instanse.TapBonusTimePercents,
            UpgradeType.TheftSpeed => Stats.Instanse.FillSpeedForHoldButton,
            UpgradeType.BackpackCapacity => Stats.Instanse.BackpackCapacity,
            _ => 0.0f,
        };
    }

    public void SetUpgradableValue(UpgradeType type, float value)
    {
        switch (type)
        {
            case UpgradeType.MoveSpeed: PlayerMovingSpeed = value; break;
            case UpgradeType.HackingSpeed: TapBonusTimePercents = value; break;
            case UpgradeType.TheftSpeed: FillSpeedForHoldButton = value; break;
            case UpgradeType.BackpackCapacity:
                BackpackCapacity = (int)value;
                UpdateCapacity();
                break;
        }
    }

    private void Awake()
    {
        Instanse = this;

    }

    private void Start()
    {
        _resourcesPanel.Initialize();

        Load();
        _resourcesPanel.SetMoney(_money);
        _xpBar.SetLevel(Level);

        foreach (var resource in _resources)
        {
            _resourcesPanel.SetResourceValue(resource.Key, resource.Value);
            _resourcesPanel.SetActiveCounter(resource.Key, resource.Value > 0);
        }

        UpdateXPBar();
        UpdateCapacity();
    }

    private void Update()
    {
        if (GameSettings.Instanse.DebugMode && Input.GetKeyDown(KeyCode.F2))
            AddXP(_neededXP - _xpAmount);
    }

    private void Load()
    {
        if (SaveLoad.HasResourcesSave)
        {
            _resources = SaveLoad.LoadResources();
            _backpackFullness = _resources.Values.Sum();
        }
        else
        {
            foreach (var resourceType in Enum.GetValues(typeof(ResourceType)))
                _resources.Add((ResourceType)resourceType, 0);
        }

        if (SaveLoad.HasLevelSave)
        {
            Level = YandexGame.savesData.Level;
            _xpAmount = YandexGame.savesData.CurrentXP;
            _neededXP = YandexGame.savesData.RequiredXP;
            NewLevelReached?.Invoke(Level);
        }
        else
        {
            Level = _initialLevel;
        }

        if (SaveLoad.HasMoneySave)
            _money = YandexGame.savesData.Money;

        _soldItemsCount = YandexGame.savesData.SoldItemsCount;
    }

    private void NextLevel()
    {
        _xpBar.SetLevel(++Level);
        _xpBar.ActiveConfetti();
        _neededXP += GameSettings.Instanse.StepXPRequirement;
        ShowQuickMessage?.Invoke($"{Translation.GetNewLevelName()}!", 3.0f);
        NewLevelReached?.Invoke(Level);
    }

    private void UpdateXPBar()
    {
        _xpBar.SetProgress(_xpAmount, _neededXP);

        if (Level >= GameSettings.Instanse.MaxLevel)
            _xpBar.SetMaxLevelState();
    }

    private void UpdateCapacity()
    {
        _resourcesPanel.SetBackpackCapacity(_backpackFullness, _backpackCapacity);
        if (BackpackIsFull)
            ShowQuickMessage?.Invoke($"{Translation.GetFullBackpackName()}!", 1.0f);
    }
}
