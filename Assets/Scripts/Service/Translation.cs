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

    private readonly static Dictionary<Language, string> _maximumName = new()
    {
        [Language.English] = "MAX",
        [Language.Russian] = "����"
    };

    public static string GetResourceName(ResourceType type, bool plural = false)
    {
        var result = _resourceNames[GameSettings.Instanse.Language][type];
        if (!plural)
            return result;
        return result + (result[^1] == 's' ? "" : "s");
    }

    public static string GetMaximumName() => _maximumName[GameSettings.Instanse.Language];
}
