using System;
using UnityEngine;
using YG;

public class Cheats : MonoBehaviour
{
    public static event Action GBuildingUpgraded;
    public static event Action<string, float, bool> GShowQuickMessage;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            GameData.Instanse.PlayerLevel.AddXPToNextLevel();
        else if (Input.GetKeyDown(KeyCode.F2) && YandexGame.savesData.TutorialDone)
            TaskManager.Instance.StartRandomTask();
        else if (Input.GetKeyDown(KeyCode.F3))
            GameData.Instanse.AddMoney(100);
        else if (Input.GetKeyDown(KeyCode.F4))
            GameData.Instanse.SetUpgradableValue(UpgradeType.BackpackCapacity, 999);
        else if (Input.GetKeyDown(KeyCode.F5))
            GBuildingUpgraded?.Invoke();
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            DayCycle.Instance.NextPhase();
            GShowQuickMessage?.Invoke(DayCycle.Instance.CurrentPhase.ToString(), 1.0f, false);
        }
    }
}
