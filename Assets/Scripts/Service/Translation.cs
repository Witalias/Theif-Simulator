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
            [EquipmentType.TierIron] = "Tier Iron"
        },
        [Language.Russian] = new Dictionary<EquipmentType, string>
        {
            [EquipmentType.Arms] = "Руки",
            [EquipmentType.Gadget] = "Гаджет",
            [EquipmentType.MasterKey] = "Отмычка",
            [EquipmentType.TierIron] = "Лом"
        }
    };

    public static string Get(LoudnessType type) => loudnessTypeNames[GameSettings.Instanse.Language][type];

    public static string Get(EquipmentType type) => equipmentTypeNames[GameSettings.Instanse.Language][type];

    public static string GetSecondsAbbreviation() => secondsAbbreviations[GameSettings.Instanse.Language];
}
