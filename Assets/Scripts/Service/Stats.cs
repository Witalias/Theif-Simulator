using UnityEngine;
using System.Collections.Generic;
using System;

public class Stats : MonoBehaviour
{
    public static Stats Instanse { get; private set; } = null;

    public static event Action<int> NewLevelReached;

    [SerializeField] private int _money = 0;
    [SerializeField] private float _playerMovingSpeed;
    [SerializeField] private float _tapBonusTimePercents = 5.0f;
    [SerializeField] private float _fillSpeedForHoldButton = 0.3f;
    [SerializeField] private int _initialLevel = 1;
    [SerializeField] private int _backpackCapacity = 2;
    [SerializeField] private int _neededXP = 3;
    [SerializeField] private ResourcesPanel _resourcesPanel;
    [SerializeField] private XPBar _xpBar;
    [SerializeField] private Transform _prisonSpawnPoint;

    private readonly Dictionary<ResourceType, int> _resources = new();
    private int _xpAmount;
    private int _backpackFullness;

    public int Level { get; private set; }
    public int Money => _money;
    public float PlayerMovingSpeed { get => _playerMovingSpeed; set => _playerMovingSpeed = value; }
    public float TapBonusTimePercents { get => _tapBonusTimePercents; set => _tapBonusTimePercents = value; }
    public float FillSpeedForHoldButton { get => _fillSpeedForHoldButton; set => _fillSpeedForHoldButton = value; }
    public int BackpackCapacity { get => _backpackCapacity; set => _backpackCapacity = value; }
    public bool BackpackIsFull => _backpackFullness >= _backpackCapacity;
    public Transform PrisonSpawnPoint => _prisonSpawnPoint;

    public void AddXP(int value)
    {
        _xpAmount += value;
        if (_xpAmount >= _neededXP)
        {
            _xpAmount -= _neededXP;
            NextLevel();
        }
        _xpBar.SetProgress(_xpAmount, _neededXP);

        if (Level >= GameSettings.Instanse.MaxLevel)
            _xpBar.SetMaxLevelState();
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
    }

    public void AddMoney(int value, bool assignTask = true)
    {
        _money = Mathf.Clamp(_money + value, 0, int.MaxValue);
        _resourcesPanel.SetMoney(_money);

        if (assignTask && value > 0)
            TaskManager.Instance.ProcessTask(TaskType.EarnMoney, value);
    }

    public int GetResourceCount(ResourceType type) => _resources[type];

    public void ClearBackpack()
    {
        foreach (var resource in Enum.GetValues(typeof(ResourceType)))
            ClearResource((ResourceType)resource);
        _resourcesPanel.ClearResources();
    }

    public void ClearResource(ResourceType type)
    {
        _backpackFullness -= _resources[type];
        _resources[type] = 0;
        _resourcesPanel.SetActiveCounter(type, false);
        UpdateCapacity();
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

    public void IncreaseUpgradableValue(UpgradeType type, float value)
    {
        switch (type)
        {
            case UpgradeType.MoveSpeed: Stats.Instanse.PlayerMovingSpeed += value; break;
            case UpgradeType.HackingSpeed: Stats.Instanse.TapBonusTimePercents += value; break;
            case UpgradeType.TheftSpeed: Stats.Instanse.FillSpeedForHoldButton += value; break;
            case UpgradeType.BackpackCapacity:
                Stats.Instanse.BackpackCapacity += (int)value;
                UpdateCapacity();
                break;
        }
    }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach (var resourceType in Enum.GetValues(typeof(ResourceType)))
            _resources.Add((ResourceType)resourceType, 0);

        Level = _initialLevel;
        _xpBar.SetLevel(_initialLevel);
    }

    private void Start()
    {
        foreach (var resource in _resources)
            _resourcesPanel.SetResourceValue(resource.Key, resource.Value);
        _resourcesPanel.SetMoney(_money);
        AddXP(0);
        UpdateCapacity();
    }

    private void NextLevel()
    {
        _xpBar.SetLevel(++Level);
        _neededXP += GameSettings.Instanse.StepXPRequirement;
        NewLevelReached?.Invoke(Level);
    }

    private void UpdateCapacity() => _resourcesPanel.SetBackpackCapacity(_backpackFullness, _backpackCapacity);
}
