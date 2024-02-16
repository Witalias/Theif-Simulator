using System.Collections.Generic;

public static class Translation
{
    private readonly static Dictionary<Language, Dictionary<ResourceType, string>> _resourceNames = new()
    {
        [Language.English] = new Dictionary<ResourceType, string>
        {
            [ResourceType.Bottle] = "Bottle",
            [ResourceType.Sneakers] = "Sneakers",
            [ResourceType.Watch] = "Watch",
            [ResourceType.Phone] = "Phone",
            [ResourceType.Ring] = "Ring",
            [ResourceType.Diamond] = "Diamond",
        },
        [Language.Russian] = new Dictionary<ResourceType, string>
        {
            [ResourceType.Bottle] = "�������",
            [ResourceType.Sneakers] = "���������",
            [ResourceType.Watch] = "����",
            [ResourceType.Phone] = "�������",
            [ResourceType.Ring] = "������",
            [ResourceType.Diamond] = "�����",
        }
    };

    private readonly static Dictionary<Language, Dictionary<ResourceType, string>> _resourceNamesPlural = new()
    {
        [Language.English] = new Dictionary<ResourceType, string>
        {
            [ResourceType.Bottle] = "Bottles",
            [ResourceType.Sneakers] = "Sneakers",
            [ResourceType.Watch] = "Watches",
            [ResourceType.Phone] = "Phones",
            [ResourceType.Ring] = "Rings",
            [ResourceType.Diamond] = "Diamonds",
        },
        [Language.Russian] = new Dictionary<ResourceType, string>
        {
            [ResourceType.Bottle] = "�������",
            [ResourceType.Sneakers] = "���������",
            [ResourceType.Watch] = "�����",
            [ResourceType.Phone] = "���������",
            [ResourceType.Ring] = "�����",
            [ResourceType.Diamond] = "�������",
        }
    };

    private readonly static Dictionary<Language, string> _maximumName = new()
    {
        [Language.English] = "MAX",
        [Language.Russian] = "����"
    };

    private readonly static Dictionary<Language, string> _completeName = new()
    {
        [Language.English] = "COMPLETED",
        [Language.Russian] = "���������"
    };

    private readonly static Dictionary<Language, string> _levelNameAbbreviated = new()
    {
        [Language.English] = "LVL",
        [Language.Russian] = "��"
    };

    private readonly static Dictionary<Language, string> _tapTapName = new()
    {
        [Language.English] = "TAP TAP",
        [Language.Russian] = "��� ���"
    };

    private readonly static Dictionary<Language, string> _clickClickName = new()
    {
        [Language.English] = "CLICK CLICK",
        [Language.Russian] = "���� ����"
    };

    private readonly static Dictionary<Language, string> _holdName = new()
    {
        [Language.English] = "HOLD",
        [Language.Russian] = "���������"
    };

    private readonly static Dictionary<Language, string> _theftName = new()
    {
        [Language.English] = "Theft",
        [Language.Russian] = "���������"
    };

    private readonly static Dictionary<Language, string> _hackingName = new()
    {
        [Language.English] = "Hacking",
        [Language.Russian] = "�����"
    };

    private readonly static Dictionary<Language, string> _increaseToName = new()
    {
        [Language.English] = "INCREASE TO",
        [Language.Russian] = "��������� ��"
    };

    private readonly static Dictionary<Language, string> _increaseToNameInPast = new()
    {
        [Language.English] = "INCREASED TO",
        [Language.Russian] = "��������� ��"
    };

    private readonly static Dictionary<Language, string> _rewardName = new()
    {
        [Language.English] = "REWARD",
        [Language.Russian] = "�������"
    };

    private readonly static Dictionary<Language, string> _noticedName = new()
    {
        [Language.English] = "NOTICED",
        [Language.Russian] = "�������"
    };

    private readonly static Dictionary<Language, string> _fullBackpackName = new()
    {
        [Language.English] = "FULL BACKPACK",
        [Language.Russian] = "������ ������"
    };

    private readonly static Dictionary<Language, string> _newLevelName = new()
    {
        [Language.English] = "NEW LEVEL",
        [Language.Russian] = "����� �������"
    };

    public static string GetResourceName(ResourceType type, bool plural = false)
    {
        return plural
            ? _resourceNamesPlural[GameSettings.Instanse.Language][type]
            : _resourceNames[GameSettings.Instanse.Language][type];
    }

    public static string GetMaximumName() => _maximumName[GameSettings.Instanse.Language];

    public static string GetCompleteName() => _completeName[GameSettings.Instanse.Language];

    public static string GetLevelNameAbbreviated() => _levelNameAbbreviated[GameSettings.Instanse.Language];

    public static string GetTapTapName() => _tapTapName[GameSettings.Instanse.Language];

    public static string GetClickClickName() => _clickClickName[GameSettings.Instanse.Language];

    public static string GetHoldName() => _holdName[GameSettings.Instanse.Language];

    public static string GetTheftName() => _theftName[GameSettings.Instanse.Language];

    public static string GetHackingName() => _hackingName[GameSettings.Instanse.Language];

    public static string GetIncreaseToName() => _increaseToName[GameSettings.Instanse.Language];

    public static string GetIncreaseToNameInPast() => _increaseToNameInPast[GameSettings.Instanse.Language];

    public static string GetRewardName() => _rewardName[GameSettings.Instanse.Language];

    public static string GetNoticedName() => _noticedName[GameSettings.Instanse.Language];

    public static string GetFullBackpackName() => _fullBackpackName[GameSettings.Instanse.Language];

    public static string GetNewLevelName() => _newLevelName[GameSettings.Instanse.Language];
}
