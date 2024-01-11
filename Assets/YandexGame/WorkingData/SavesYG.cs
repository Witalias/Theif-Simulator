
using System.Collections.Generic;

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

        public int Level = -1;
        public int CurrentXP;
        public int RequiredXP;
        public int Money = -1;
        public List<string> ResourceData = new();
        public List<string> UpgradeData = new();
        public List<string> BuildingData = new();
        public List<string> UnlockAreaData = new();
        public string PlayerPosition;

        public string TaskType;
        public string RequiredResource;
        public int CurrentTaskProgress;
        public int TaskRequirement;
        public int TaskReward;
    }
}
