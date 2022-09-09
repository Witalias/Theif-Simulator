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
            [LoudnessType.Loudly] = "громко",
            [LoudnessType.Quietly] = "тихо",
            [LoudnessType.VeryLoudly] = "очень громко"
        }
    };

    private readonly static Dictionary<Language, string> secondsAbbreviations = new Dictionary<Language, string>
    {
        [Language.English] = "sec",
        [Language.Russian] = "сек"
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
            [EquipmentType.Arms] = "Руки",
            [EquipmentType.Gadget] = "Гаджет",
            [EquipmentType.MasterKey] = "Отмычка",
            [EquipmentType.TierIron] = "Лом"
        }
    };

    private readonly static Dictionary<Language, string> noiseName = new Dictionary<Language, string>
    {
        [Language.English] = "Noise",
        [Language.Russian] = "Пошуметь"
    };

    private readonly static Dictionary<Language, Dictionary<ResourceType, string>> resourceNames = new Dictionary<Language, Dictionary<ResourceType, string>>
    {
        [Language.English] = new Dictionary<ResourceType, string>
        {
            [ResourceType.Food] = "Food",
            [ResourceType.Fuel] = "Fuel",
            [ResourceType.MasterKeys] = "Master keys",
            [ResourceType.Money] = "Money",
            [ResourceType.TierIrons] = "Crowbars",
            [ResourceType.Water] = "Water",
            [ResourceType.Gadgets] = "Gadgets"
        },
        [Language.Russian] = new Dictionary<ResourceType, string>
        {
            [ResourceType.Food] = "Еда",
            [ResourceType.Fuel] = "Топливо",
            [ResourceType.MasterKeys] = "Отмычки",
            [ResourceType.Money] = "Монеты",
            [ResourceType.TierIrons] = "Лом",
            [ResourceType.Water] = "Вода",
            [ResourceType.Gadgets] = "Гаджеты"
        }
    };

    private readonly static Dictionary<Language, string> visibilityNames = new Dictionary<Language, string>
    {
        [Language.English] = "Visibility",
        [Language.Russian] = "Заметность"
    };

    private readonly static Dictionary<Language, string> visibilityLevelNames = new Dictionary<Language, string>
    {
        [Language.English] = "Visibility level",
        [Language.Russian] = "Уровень заметности"
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
            [VisibilityEventType.DoubleLocks] = "Жильцы установили двойные замки на все запертые двери и окна!",
            [VisibilityEventType.ExtraResident] = "В дом прибыл ещё один жилец!",
            [VisibilityEventType.IncreasedRangeAndViewAngle] = "Жильцы взволнованы, их дальность и угол обзора увеличены!",
            [VisibilityEventType.IncreasedSpeed] = "Жильцы взволнованы и ходят быстрее!",
            [VisibilityEventType.LockDoorsAndWindows] = "Жильцы заперли некоторые двери и окна в доме!",
            [VisibilityEventType.NewPoliceman] = "Прибыл ещё один полицейский!",
            [VisibilityEventType.NoReactionToNoise] = "Жильцы больше не реагируют на намеренно издаваемый шум!",
            [VisibilityEventType.Police] = "Жильцы вызвали полицию!",
            [VisibilityEventType.SequrityCameras] = "Жильцы установили камеры наблюдения в доме!",
            [VisibilityEventType.SoundDetectors] = "Жильцы установили звуковые датчики в доме!",
            [VisibilityEventType.Traps] = "Жильцы установили ловушки на некоторые предметы мебели!",
            [VisibilityEventType.MoreIncreasedSpeed] = "Жильцы взволнованы и ходят ЕЩЁ БЫСТРЕЕ!",
            [VisibilityEventType.IncreasedHearingRadius] = "Жильцы настороже и теперь прислушиваются к каждому шороху!"
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
            [SkillType.SlyFox] = "The sly fox"
        },
        [Language.Russian] = new Dictionary<SkillType, string>
        {
            [SkillType.FastFeet] = "Быстрые ноги",
            [SkillType.NotCheackingDoors] = "НЕ скрипучие двери",
            [SkillType.PracticeOfHackingWithACrowbar] = "Практика взлома ломом",
            [SkillType.PracticeOfHackingWithAMasterKey] = "Практика взлома отмычкой",
            [SkillType.PracticeOfManualHacking] = "Практика ручного взлома",
            [SkillType.PracticeOfHackingWithAGadget] = "Практика взлома гаджетом",
            [SkillType.SlyFox] = "Хитрая лиса"
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
            [SkillType.SlyFox] = "Ability to make noise and attract attention"
        },
        [Language.Russian] = new Dictionary<SkillType, string>
        {
            [SkillType.FastFeet] = "Увеличение скорости передвижения",
            [SkillType.NotCheackingDoors] = "Уменьшение шума при открытии дверей",
            [SkillType.PracticeOfHackingWithACrowbar] = "Уменьшение времени взлома ломом",
            [SkillType.PracticeOfHackingWithAMasterKey] = "Уменьшение времени взлома отмычкой",
            [SkillType.PracticeOfManualHacking] = "Уменьшение времени взлома руками",
            [SkillType.PracticeOfHackingWithAGadget] = "Уменьшение времени взлома гаджетом",
            [SkillType.SlyFox] = "Способность шуметь и привлекать внимание"
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
