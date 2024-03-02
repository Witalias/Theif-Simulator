using System.Collections.Generic;
using YG;

public static class MetricaSender
{
    public static void UnlockedArea(int cost)
    {
        YandexMetrica.Send("UnlockedAreas", new Dictionary<string, string> { { "�������� ����",  $"����: {cost}" } });
    }

    public static void PlayerUpgrade(UpgradeType type, int level)
    {
        YandexMetrica.Send("PlayerUpgrades", new Dictionary<string, string> { { "������ ��������� �����", $"{type} - {level}" } });
    }

    public static void BuildingUpgrade(string name, int level)
    {
        YandexMetrica.Send("BuildingUpgrades", new Dictionary<string, string> { { "������ ������", $"{name} - {level}" } });
    }

    public static void PlayerLevel(int level)
    {
        YandexMetrica.Send("PlayerLevel", new Dictionary<string, string> { { "������ �����", level.ToString() } });
    }

    public static void TutorialStep(string name)
    {
        YandexMetrica.Send("TutorialSteps", new Dictionary<string, string> { { "���� ��������", name } });
    }
}
