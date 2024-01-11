using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

public static class SaveLoad
{
    public static bool HasLevelSave => YandexGame.savesData.Level >= 0;
    public static bool HasMoneySave => YandexGame.savesData.Money >= 0;
    public static bool HasResourcesSave => YandexGame.savesData.ResourceData.Count > 0;
    public static bool HasUpgradesSave => YandexGame.savesData.UpgradeData.Count > 0;
    public static bool HasBuildingsSave => YandexGame.savesData.BuildingData.Count > 0;
    public static bool HasUnlockAreasSave => YandexGame.savesData.UnlockAreaData.Count > 0;
    public static bool HasPlayerPositionSave => YandexGame.savesData.PlayerPosition != null;
    public static bool HasTaskSave => YandexGame.savesData.TaskType != null;

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

    public static void SaveTutorialLootSafeDoneBoolean(bool value)
    {
        YandexGame.savesData.TutorialLootSafeDone = value;
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

    public static void SaveSoldItemsCount(int value)
    {
        YandexGame.savesData.SoldItemsCount = value;
        YandexGame.SaveProgress();
    }

    public static void SaveTask(TaskType taskType, ResourceType requiredResource, int progress, int requirement, int reward)
    {
        YandexGame.savesData.TaskType = taskType.ToString();
        YandexGame.savesData.RequiredResource = requiredResource.ToString();
        YandexGame.savesData.CurrentTaskProgress = progress;
        YandexGame.savesData.TaskRequirement = requirement;
        YandexGame.savesData.TaskReward = reward;
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

    public static void SaveUpgradeLevels(IEnumerable<Upgrade> upgrades)
    {
        YandexGame.savesData.UpgradeData.Clear();
        foreach (var upgrade in upgrades)
        {
            var data = $"{upgrade.Type}:{upgrade.Level}";
            YandexGame.savesData.UpgradeData.Add(data);
        }
        YandexGame.SaveProgress();       
    }

    public static void SaveBuildings(IEnumerable<Building.SavedData> buildings)
    {
        YandexGame.savesData.BuildingData.Clear();
        foreach (var building in buildings)
            YandexGame.savesData.BuildingData.Add(JsonUtility.ToJson(building));
        YandexGame.SaveProgress();
    }

    public static void SaveUnlockAreas(IEnumerable<UnlockArea.SavedData> unlockAreas)
    {
        YandexGame.savesData.UnlockAreaData.Clear();
        foreach (var unlockArea in unlockAreas)
            YandexGame.savesData.UnlockAreaData.Add(JsonUtility.ToJson(unlockArea));
        YandexGame.SaveProgress();
    }

    public static void SavePlayerPosition(Vector3 position)
    {
        YandexGame.savesData.PlayerPosition = JsonUtility.ToJson(position);
        YandexGame.SaveProgress();
    }

    public static Dictionary<ResourceType, int> LoadResources()
    {
        return YandexGame.savesData.ResourceData
                .Select(data => data.Split(':'))
                .ToDictionary(data => Enum.Parse<ResourceType>(data[0]), data => int.Parse(data[1]));
    }

    public static Dictionary<UpgradeType, int> LoadUpgradeLevels()
    {
        return YandexGame.savesData.UpgradeData
                .Select(data => data.Split(':'))
                .ToDictionary(data => Enum.Parse<UpgradeType>(data[0]), data => int.Parse(data[1]));
    }

    public static Dictionary<int, Building.SavedData> LoadBuildings()
    {
        return YandexGame.savesData.BuildingData
            .Select(data => JsonUtility.FromJson<Building.SavedData>(data))
            .ToDictionary(data => data.ID);
    }

    public static Dictionary<int, UnlockArea.SavedData> LoadUnlockAreas()
    {
        return YandexGame.savesData.UnlockAreaData
            .Select(data => JsonUtility.FromJson<UnlockArea.SavedData>(data))
            .ToDictionary(data => data.ID);
    }

    public static Vector3 LoadPlayerPosition()
    {
        return JsonUtility.FromJson<Vector3>(YandexGame.savesData.PlayerPosition);
    }
}
