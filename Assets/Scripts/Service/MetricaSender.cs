using System.Collections.Generic;
using YG;

public static class MetricaSender
{
    public static void UnlockedArea(int cost)
    {
        YandexMetrica.Send("UnlockedAreas", new Dictionary<string, string> { { "UnlockedAreas", cost.ToString() } });
    }

    public static void PlayerUpgrade(UpgradeType type, int level)
    {
        YandexMetrica.Send("PlayerUpgrades", new Dictionary<string, string> { { "PlayerUpgrades", $"{type}: {level}" } });
    }

    public static void BuildingUpgrade(string name, int level)
    {
        YandexMetrica.Send("BuildingUpgrades", new Dictionary<string, string> { { "BuildingUpgrades", $"{name}: {level}" } });
    }

    public static void PlayerLevel(int level)
    {
        YandexMetrica.Send("Level", new Dictionary<string, string> { { "Level", level.ToString() } });
    }
}
