using System;
using System.Collections.Generic;
using System.Linq;
using YG;

public static class SaveLoad
{
    public static bool HasLevelSave => YandexGame.savesData.Level >= 0;
    public static bool HasMoneySave => YandexGame.savesData.Money >= 0;
    public static bool HasResourcesSave => YandexGame.savesData.ResourceData.Count > 0;
    public static bool HasUpgradesSave => YandexGame.savesData.UpgradeData.Count > 0;

    public static void SaveTutorialDoneBoolean(bool value)
    {
        YandexGame.savesData.TutorialDone = value;
        YandexGame.SaveProgress();
    }

    public static void SaveTutorialCrackDoorDoneBoolean(bool value)
    {
        YandexGame.savesData.TutorialCrackDoorDone = value;
        YandexGame.SaveProgress();
    }

    public static void SaveTutorialRobHouseDoneBoolean(bool value)
    {
        YandexGame.savesData.TutorialRobHouseDone = value;
        YandexGame.SaveProgress();
    }

    public static void SaveTutorialSellItemsDoneBoolean(bool value)
    {
        YandexGame.savesData.TutorialSellItemsDone = value;
        YandexGame.SaveProgress();
    }

    public static void SaveTutorialBuyUpgradeDoneBoolean(bool value)
    {
        YandexGame.savesData.TutorialBuyUpgradeDone = value;
        YandexGame.SaveProgress();
    }

    public static void SaveTutorialBuyZoneDoneBoolean(bool value)
    {
        YandexGame.savesData.TutorialBuyZoneDone = value;
        YandexGame.SaveProgress();
    }

    public static void SaveTutorialEnterBuildingWithEnemyDoneBoolean(bool value)
    {
        YandexGame.savesData.TutorialEnterBuildingWithEnemyDone = value;
        YandexGame.SaveProgress();
    }

    public static void SaveLevel(int value, int currentXP, int requiredXP)
    {
        YandexGame.savesData.Level = value;
        YandexGame.savesData.CurrentXP = currentXP;
        YandexGame.savesData.RequiredXP = requiredXP;
        YandexGame.SaveProgress();
    }

    public static void SaveMoney(int value)
    {
        YandexGame.savesData.Money = value;
        YandexGame.SaveProgress();
    }

    public static void SaveResources(Dictionary<ResourceType, int> resources)
    {
        YandexGame.savesData.ResourceData.Clear();
        foreach (var resource in resources)
        {
            var data = $"{resource.Key}:{resource.Value}";
            YandexGame.savesData.ResourceData.Add(data);
        }
        YandexGame.SaveProgress();
    }

    public static void SaveUpgradeLevels(Upgrade[] upgrades)
    {
        YandexGame.savesData.UpgradeData.Clear();
        foreach (var upgrade in upgrades)
        {
            var data = $"{upgrade.Type}:{upgrade.Level}";
            YandexGame.savesData.UpgradeData.Add(data);
        }
        YandexGame.SaveProgress();       
    }

    public static Dictionary<ResourceType, int> LoadResources()
    {
        if (YandexGame.savesData.ResourceData.Count == 0)
            return null;

        return YandexGame.savesData.ResourceData
                .Select(data => data.Split(':'))
                .ToDictionary(data => Enum.Parse<ResourceType>(data[0]), data => int.Parse(data[1]));
    }

    public static Dictionary<UpgradeType, int> LoadUpgradeLevels()
    {
        if (YandexGame.savesData.UpgradeData.Count == 0)
            return null;

        return YandexGame.savesData.UpgradeData
                .Select(data => data.Split(':'))
                .ToDictionary(data => Enum.Parse<UpgradeType>(data[0]), data => int.Parse(data[1]));
    }
}
