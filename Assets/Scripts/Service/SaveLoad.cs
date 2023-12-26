using YG;

public static class SaveLoad
{
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
}
