
namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        public bool TutorialDone;
        public bool TutorialCrackDoorDone;
        public bool TutorialRobHouseDone;
        public bool TutorialSellItemsDone;
        public bool TutorialBuyUpgradeDone;
        public bool TutorialBuyZoneDone;
        public bool TutorialEnterBuildingWithEnemyDone;
    }
}
