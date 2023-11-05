using System.Collections.Generic;

public static class Translation
{
    private readonly static Dictionary<Language, Dictionary<LoudnessType, string>> loudnessTypeNames = new Dictionary<Language, Dictionary<LoudnessType, string>>
    {
        [Language.English] = new Dictionary<LoudnessType, string>
        {
            [LoudnessType.Loudly] = "loudly",
            [LoudnessType.Quietly] = "quietly",
            [LoudnessType.VeryLoudly] = "very loudly"
        },
        [Language.Russian] = new Dictionary<LoudnessType, string>
        {
            [LoudnessType.Loudly] = "������",
            [LoudnessType.Quietly] = "����",
            [LoudnessType.VeryLoudly] = "����� ������"
        }
    };

    private readonly static Dictionary<Language, string> secondsAbbreviations = new Dictionary<Language, string>
    {
        [Language.English] = "sec",
        [Language.Russian] = "���"
    };

    private readonly static Dictionary<Language, Dictionary<EquipmentType, string>> equipmentTypeNames = new Dictionary<Language, Dictionary<EquipmentType, string>>
    {
        [Language.English] = new Dictionary<EquipmentType, string>
        {
            [EquipmentType.Arms] = "Arms",
            [EquipmentType.Gadget] = "Gadget",
            [EquipmentType.MasterKey] = "Master key",
            [EquipmentType.TierIron] = "Crowbar"
        },
        [Language.Russian] = new Dictionary<EquipmentType, string>
        {
            [EquipmentType.Arms] = "����",
            [EquipmentType.Gadget] = "������",
            [EquipmentType.MasterKey] = "�������",
            [EquipmentType.TierIron] = "���"
        }
    };

    private readonly static Dictionary<Language, string> noiseName = new Dictionary<Language, string>
    {
        [Language.English] = "Noise",
        [Language.Russian] = "��������"
    };

    private readonly static Dictionary<Language, Dictionary<ResourceType, string>> resourceNames = new Dictionary<Language, Dictionary<ResourceType, string>>
    {
        [Language.English] = new Dictionary<ResourceType, string>
        {
            [ResourceType.Bootle] = "Bootle",
            [ResourceType.Sneakers] = "Sneakers",
        },
        [Language.Russian] = new Dictionary<ResourceType, string>
        {
            [ResourceType.Bootle] = "Bootle",
            [ResourceType.Sneakers] = "Sneakers",
        }
    };

    private readonly static Dictionary<Language, string> visibilityNames = new Dictionary<Language, string>
    {
        [Language.English] = "Visibility",
        [Language.Russian] = "����������"
    };

    private readonly static Dictionary<Language, string> visibilityLevelNames = new Dictionary<Language, string>
    {
        [Language.English] = "Visibility level",
        [Language.Russian] = "������� ����������"
    };

    private readonly static Dictionary<Language, Dictionary<VisibilityEventType, string>> visibilityEventMessages = new Dictionary<Language, Dictionary<VisibilityEventType, string>>
    {
        [Language.English] = new Dictionary<VisibilityEventType, string>
        {
            [VisibilityEventType.DoubleLocks] = "Residents put double locks on all locked doors and windows!",
            [VisibilityEventType.ExtraResident] = "A new resident has arrived at the house!",
            [VisibilityEventType.IncreasedRangeAndViewAngle] = "Residents are excited, their range and viewing angle are increased!",
            [VisibilityEventType.IncreasedSpeed] = "Residents are excited and walk faster!",
            [VisibilityEventType.LockDoorsAndWindows] = "Residents locked some doors and windows in the house!",
            [VisibilityEventType.NewPoliceman] = "A new policeman has arrived!",
            [VisibilityEventType.NoReactionToNoise] = "Residents no longer react to intentionally made noise!",
            [VisibilityEventType.Police] = "Residents called the police!",
            [VisibilityEventType.SequrityCameras] = "Residents installed security cameras in the house!",
            [VisibilityEventType.SoundDetectors] = "Residents installed sound detectors in the house!",
            [VisibilityEventType.Traps] = "Residents put traps on some furniture!",
            [VisibilityEventType.MoreIncreasedSpeed] = "Residents are excited and walk EVEN FASTER!",
            [VisibilityEventType.IncreasedHearingRadius] = "Residents are on the alert and now listen to every rustle!"
        },
        [Language.Russian] = new Dictionary<VisibilityEventType, string>
        {
            [VisibilityEventType.DoubleLocks] = "������ ���������� ������� ����� �� ��� �������� ����� � ����!",
            [VisibilityEventType.ExtraResident] = "� ��� ������ ��� ���� �����!",
            [VisibilityEventType.IncreasedRangeAndViewAngle] = "������ �����������, �� ��������� � ���� ������ ���������!",
            [VisibilityEventType.IncreasedSpeed] = "������ ����������� � ����� �������!",
            [VisibilityEventType.LockDoorsAndWindows] = "������ ������� ��������� ����� � ���� � ����!",
            [VisibilityEventType.NewPoliceman] = "������ ��� ���� �����������!",
            [VisibilityEventType.NoReactionToNoise] = "������ ������ �� ��������� �� ��������� ���������� ���!",
            [VisibilityEventType.Police] = "������ ������� �������!",
            [VisibilityEventType.SequrityCameras] = "������ ���������� ������ ���������� � ����!",
            [VisibilityEventType.SoundDetectors] = "������ ���������� �������� ������� � ����!",
            [VisibilityEventType.Traps] = "������ ���������� ������� �� ��������� �������� ������!",
            [VisibilityEventType.MoreIncreasedSpeed] = "������ ����������� � ����� �٨ �������!",
            [VisibilityEventType.IncreasedHearingRadius] = "������ ��������� � ������ �������������� � ������� ������!"
        }
    };

    private readonly static Dictionary<Language, Dictionary<SkillType, string>> skillTitles = new Dictionary<Language, Dictionary<SkillType, string>>
    {
        [Language.English] = new Dictionary<SkillType, string>
        {
            [SkillType.FastFeet] = "Fast feet",
            [SkillType.NotCheackingDoors] = "NOT squeaky doors",
            [SkillType.PracticeOfHackingWithACrowbar] = "The practice of hacking with a crowbar",
            [SkillType.PracticeOfHackingWithAMasterKey] = "The practice of hacking with a master key",
            [SkillType.PracticeOfManualHacking] = "The practice of manual hacking",
            [SkillType.PracticeOfHackingWithAGadget] = "The practice of hacking with a gadget",
            [SkillType.SlyFox] = "The sly fox",
            [SkillType.PracticeOfHacking] = "The practice of hacking",
            [SkillType.AgilityOfManualHacking] = "Agility of manual hacking",
            [SkillType.AgilityOfHackingWithACrowbar] = "Agility of hacking with a crowbar",
            [SkillType.AgilityOfHacking] = "Agility of hacking",
            [SkillType.MoreFood] = "A skilled food seeker",
            [SkillType.MoreWater] = "A skilled water seeker",
            [SkillType.MoreMoney] = "A skilled money seeker",
            [SkillType.MoreFuel] = "A skilled fuel seeker",
            [SkillType.LessVisibilityScale] = "Inconspicuous",
            [SkillType.NoVisibilityFromIntentionalNoise] = "The secretive fox",
            [SkillType.SkilledToolFinder] = "A skilled tool finder",
            [SkillType.SkilledMasterKeysFinder] = "Key search master",
            [SkillType.SkilledTierIronsFinder] = "Crowbar search master",
            [SkillType.SkilledGadgetsFinder] = "Gadget search master",
        },
        [Language.Russian] = new Dictionary<SkillType, string>
        {
            [SkillType.FastFeet] = "������� ����",
            [SkillType.NotCheackingDoors] = "�� ��������� �����",
            [SkillType.PracticeOfHackingWithACrowbar] = "�������� ������ �����",
            [SkillType.PracticeOfHackingWithAMasterKey] = "�������� ������ ��������",
            [SkillType.PracticeOfManualHacking] = "�������� ������� ������",
            [SkillType.PracticeOfHackingWithAGadget] = "�������� ������ ��������",
            [SkillType.SlyFox] = "������ ���",
            [SkillType.PracticeOfHacking] = "�������� ������",
            [SkillType.AgilityOfManualHacking] = "�������� ������� ������",
            [SkillType.AgilityOfHackingWithACrowbar] = "�������� ������ �����",
            [SkillType.AgilityOfHacking] = "�������� ������",
            [SkillType.MoreFood] = "������ �������� ���",
            [SkillType.MoreWater] = "������ �������� ����",
            [SkillType.MoreMoney] = "������ �������� �����",
            [SkillType.MoreFuel] = "������ �������� �������",
            [SkillType.LessVisibilityScale] = "����������",
            [SkillType.NoVisibilityFromIntentionalNoise] = "�������� ���",
            [SkillType.SkilledToolFinder] = "������ �������� ������������",
            [SkillType.SkilledMasterKeysFinder] = "������ ������ �������",
            [SkillType.SkilledTierIronsFinder] = "������ ������ �����",
            [SkillType.SkilledGadgetsFinder] = "������ ������ ��������",
        }
    };

    private readonly static Dictionary<Language, Dictionary<SkillType, string>> skillDescriptions = new Dictionary<Language, Dictionary<SkillType, string>>
    {
        [Language.English] = new Dictionary<SkillType, string>
        {
            [SkillType.FastFeet] = "Increased movement speed",
            [SkillType.NotCheackingDoors] = "Noise reduction when opening doors",
            [SkillType.PracticeOfHackingWithACrowbar] = "Reducing the time of hacking with a crowbar",
            [SkillType.PracticeOfHackingWithAMasterKey] = "Reducing the time of hacking with a master key",
            [SkillType.PracticeOfManualHacking] = "Reducing the time of hacking by arms",
            [SkillType.PracticeOfHackingWithAGadget] = "Reducing the time of hacking with a gadget",
            [SkillType.SlyFox] = "Ability to make noise and attract attention",
            [SkillType.PracticeOfHacking] = "Reducing the time of hacking by any tool",
            [SkillType.AgilityOfManualHacking] = "Reducing noise when hacking by arms",
            [SkillType.AgilityOfHackingWithACrowbar] = "Reducing noise when hacking by crowbar",
            [SkillType.AgilityOfHacking] = "Reducing noise when hacking by any tool",
            [SkillType.MoreFood] = "Increasing the food found to",
            [SkillType.MoreWater] = "Increasing the water found to",
            [SkillType.MoreMoney] = "Increasing the money found to",
            [SkillType.MoreFuel] = "Increasing the fuel found to",
            [SkillType.LessVisibilityScale] = "Reducing the speed of self-filling the visibility scale",
            [SkillType.NoVisibilityFromIntentionalNoise] = "Intentional noise does not increase the visibility scale",
            [SkillType.SkilledToolFinder] = "Increasing the chance of finding tools",
            [SkillType.SkilledMasterKeysFinder] = "Chance to find 2 master keys instead of one",
            [SkillType.SkilledTierIronsFinder] = "Chance to find 2 crowbars instead of one",
            [SkillType.SkilledGadgetsFinder] = "Chance to find 2 gadgets instead of one",
        },

        [Language.Russian] = new Dictionary<SkillType, string>
        {
            [SkillType.FastFeet] = "���������� �������� ������������",
            [SkillType.NotCheackingDoors] = "���������� ���� ��� �������� ������",
            [SkillType.PracticeOfHackingWithACrowbar] = "���������� ������� ������ �����",
            [SkillType.PracticeOfHackingWithAMasterKey] = "���������� ������� ������ ��������",
            [SkillType.PracticeOfManualHacking] = "���������� ������� ������ ������",
            [SkillType.PracticeOfHackingWithAGadget] = "���������� ������� ������ ��������",
            [SkillType.SlyFox] = "����������� ������ � ���������� ��������",
            [SkillType.PracticeOfHacking] = "���������� ������� ������ ����� ������������",
            [SkillType.AgilityOfManualHacking] = "���������� ���� ��� ������ ������",
            [SkillType.AgilityOfHackingWithACrowbar] = "���������� ���� ��� ������ �����",
            [SkillType.AgilityOfHacking] = "���������� ���� ��� ������ ����� ������������",
            [SkillType.MoreFood] = "���������� ��������� ��� � ������ ��",
            [SkillType.MoreWater] = "���������� ��������� ���� � ������ ��",
            [SkillType.MoreMoney] = "���������� ��������� ����� � ������ ��",
            [SkillType.MoreFuel] = "���������� ���������� ������� � ������ ��",
            [SkillType.LessVisibilityScale] = "���������� �������� ���������������� ���������� ����� ����������",
            [SkillType.NoVisibilityFromIntentionalNoise] = "���������� ��� �� ����������� ����� ����������",
            [SkillType.SkilledToolFinder] = "���������� ����� ���������� ������������",
            [SkillType.SkilledMasterKeysFinder] = "���� ����� 2 ������� ������ �����",
            [SkillType.SkilledTierIronsFinder] = "���� ����� 2 ���� ������ ������",
            [SkillType.SkilledGadgetsFinder] = "���� ����� 2 ������� ������ ������",
        }
    };

    public static string Get(LoudnessType type) => loudnessTypeNames[GameSettings.Instanse.Language][type];

    public static string Get(EquipmentType type) => equipmentTypeNames[GameSettings.Instanse.Language][type];

    public static string Get(ResourceType type) => resourceNames[GameSettings.Instanse.Language][type];

    public static string Get(VisibilityEventType type) => visibilityEventMessages[GameSettings.Instanse.Language][type];

    public static string GetTitle(SkillType type) => skillTitles[GameSettings.Instanse.Language][type];

    public static string GetDescription(SkillType type) => skillDescriptions[GameSettings.Instanse.Language][type];

    public static string GetSecondsAbbreviation() => secondsAbbreviations[GameSettings.Instanse.Language];

    public static string GetNoiseName() => noiseName[GameSettings.Instanse.Language];

    public static string GetVisibilityName() => visibilityNames[GameSettings.Instanse.Language];

    public static string GetVisibilityLevelName() => visibilityLevelNames[GameSettings.Instanse.Language];

}
