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
            [ResourceType.Bottle] = "Бутылка",
            [ResourceType.Sneakers] = "Кроссовки",
            [ResourceType.Watch] = "Часы",
            [ResourceType.Phone] = "Телефон",
            [ResourceType.Ring] = "Кольцо",
            [ResourceType.Diamond] = "Алмаз",
        }
    };

    private readonly static Dictionary<Language, string> _maximumName = new()
    {
        [Language.English] = "MAX",
        [Language.Russian] = "МАКС"
    };

    private readonly static Dictionary<Language, string> _completeName = new()
    {
        [Language.English] = "COMPLETED",
        [Language.Russian] = "ЗАВЕРШЕНО"
    };

    public static string GetResourceName(ResourceType type, bool plural = false)
    {
        var result = _resourceNames[GameSettings.Instanse.Language][type];
        if (!plural)
            return result;
        return result + (result[^1] == 's' ? "" : "s");
    }

    public static string GetMaximumName() => _maximumName[GameSettings.Instanse.Language];

    public static string GetCompleteName() => _completeName[GameSettings.Instanse.Language];
}
