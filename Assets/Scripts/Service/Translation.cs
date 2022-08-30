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

    public static string Get(LoudnessType type) => loudnessTypeNames[GameSettings.Instanse.Language][type];

    public static string Get(EquipmentType type) => equipmentTypeNames[GameSettings.Instanse.Language][type];

    public static string Get(ResourceType type) => resourceNames[GameSettings.Instanse.Language][type];

    public static string GetSecondsAbbreviation() => secondsAbbreviations[GameSettings.Instanse.Language];

    public static string GetNoiseName() => noiseName[GameSettings.Instanse.Language];

    public static string GetVisibilityName() => visibilityNames[GameSettings.Instanse.Language];

}
