using UnityEngine;
using YG;

public class Cheats : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            GameData.Instanse.PlayerLevel.AddXPToNextLevel();
        else if (Input.GetKeyDown(KeyCode.F2) && YandexGame.savesData.TutorialDone)
            TaskManager.Instance.StartRandomTask();
        else if (Input.GetKeyDown(KeyCode.F3))
            GameData.Instanse.AddMoney(100);
    }
}
