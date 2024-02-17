using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using YG;

public class GameData : MonoBehaviour
{
    public static GameData Instanse { get; private set; } = null;

    [SerializeField] private ResourceInfo[] _resourceData;

    [Header("Language")]
    [SerializeField] private bool _loadLanguageFromYG;
    [SerializeField] private Language _language = Language.Russian;

    [Header("Properties")]
    [SerializeField] private float _appearHackingZonesRadius = 20.0f;
    [SerializeField] private int _stepXPRequirement = 2;
    [SerializeField] private int _initialXPRequirement;
    [SerializeField] private int _maxLevel = 30;
    [SerializeField] private int _hackingXPReward;
    [SerializeField] private int _theftXPReward;

    [Header("Leaderboard")]
    [SerializeField] private string _leaderboardName;

    [Header("Points")]
    [SerializeField] private Transform _prisonSpawnPoint;
    [SerializeField] private Transform _initialPlayerSpawnPoint;

    [Header("Scene")]
    [SerializeField] private ResourcesPanel _resourcesPanel;
    [SerializeField] private XPBar _xpBar;

    private Dictionary<ResourceType, ResourceInfo> _resources;
    private float _playerMovingSpeed;
    private float _tapBonusTimePercents;
    private float _fillSpeedForHoldButton;
    private int _soldItemsCount;

    public Language Language { get => _language; set => _language = value; }
    public BackpackController Backpack { get; private set; }
    public PlayerLevelController PlayerLevel { get; private set; }
    public int Money { get; private set; }
    public float AppearHackingZonesDistance => _appearHackingZonesRadius;
    public int StepXPRequirement => _stepXPRequirement;
    public int MaxLevel => _maxLevel;
    public int HackingXPReward => _hackingXPReward;
    public int TheftXPReward => _theftXPReward;
    public Transform PrisonSpawnPoint => _prisonSpawnPoint;
    public Transform InitialPlayerSpawnPoint => _initialPlayerSpawnPoint;
    public string LeaderboardName => _leaderboardName;
    public float PlayerMovingSpeed => _playerMovingSpeed;
    public float TapBonusTimePercents => _tapBonusTimePercents;
    public float FillSpeedForHoldButton => _fillSpeedForHoldButton;

    private void Awake()
    {
        Instanse = this;
        _resources = _resourceData.ToDictionary(resource => resource.Type);
        Backpack = new(_resourcesPanel);
        PlayerLevel = new(_xpBar, _initialXPRequirement);

        if (SaveLoad.HasMoneySave)
            Money = YandexGame.savesData.Money;
        Backpack.SetMoney(Money);

        _soldItemsCount = YandexGame.savesData.SoldItemsCount;

        if (_loadLanguageFromYG)
            LoadLanguage();

        if (YandexGame.EnvironmentData.payload == "delete_save")
        {
            YandexGame.ResetSaveProgress();
            YandexGame.SaveProgress();
        }
    }

    public Sprite GetResourceSprite(ResourceType type) => _resources[type].Sprite;

    public Sound GetResourceSound(ResourceType type) => _resources[type].Sound;

    public int GetResourcePrice(ResourceType type) => _resources[type].Price;

    public float GetUpgradableValue(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.MoveSpeed => PlayerMovingSpeed,
            UpgradeType.HackingSpeed => TapBonusTimePercents,
            UpgradeType.TheftSpeed => FillSpeedForHoldButton,
            UpgradeType.BackpackCapacity => Backpack.Capacity,
            _ => 0.0f,
        };
    }

    public void SetUpgradableValue(UpgradeType type, float value)
    {
        switch (type)
        {
            case UpgradeType.MoveSpeed: _playerMovingSpeed = value; break;
            case UpgradeType.HackingSpeed: _tapBonusTimePercents = value; break;
            case UpgradeType.TheftSpeed: _fillSpeedForHoldButton = value; break;
            case UpgradeType.BackpackCapacity: Backpack.SetCapacity((int)value); break;
        }
    }

    public void AddSoldItemsCount(int value)
    {
        _soldItemsCount += value;
        SaveLoad.SaveSoldItemsCount(_soldItemsCount);
        YandexGame.NewLeaderboardScores(GameData.Instanse.LeaderboardName, _soldItemsCount);
    }

    public void AddMoney(int value, bool assignTask = true)
    {
        Money = Mathf.Clamp(Money + value, 0, int.MaxValue);
        GameData.Instanse.Backpack.SetMoney(Money);

        if (assignTask && value > 0)
            TaskManager.Instance.ProcessTask(TaskType.EarnMoney, value);

        SaveLoad.SaveMoney(Money);
    }

    private void LoadLanguage()
    {
        Language = YandexGame.EnvironmentData.language switch
        {
            "ru" => Language.Russian,
            _ => Language.English,
        };
    }
}
