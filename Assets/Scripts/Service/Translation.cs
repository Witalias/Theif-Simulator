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

    public static string GetResourceName(ResourceType type) => _resourceNames[GameSettings.Instanse.Language][type];
}
