using System.Collections.Generic;

public static class Translation
{
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
}
