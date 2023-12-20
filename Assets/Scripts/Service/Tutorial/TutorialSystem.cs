using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class TutorialSystem : MonoBehaviour
{
    [SerializeField] private float _startingTaskDelay;
    [SerializeField] private GameObject _arrow3dPrefab;
    [SerializeField] private GameObject _arrow2d;

    [Header("Arrow points")]
    [SerializeField] private Transform _crackDoorArrowPoint;
    [SerializeField] private Transform[] _lootableAreaPoints;
    [SerializeField] private Transform _sellItemsPoint;
    [SerializeField] private Transform _upgradePanelButton;
    [SerializeField] private Transform _unlockArea;

    [Header("Walls")]
    [SerializeField] private GameObject _robWalls;
    [SerializeField] private GameObject _sellWalls;

    private readonly List<Transform> _arrows = new();

    private void Start()
    {
        if (YandexGame.savesData.TutorialDone)
            return;

        if (!YandexGame.savesData.TutorialCrackDoorDone)
            StartCrackDoorTututial();
        else if (!YandexGame.savesData.TutorialRobHouseDone)
            StartRobHouseTutorial();
        else if (!YandexGame.savesData.TutorialSellItemsDone)
            StartSellItemsTutorial();
        else if (!YandexGame.savesData.TutorialBuyUpgradeDone)
            StartBuyUpgradeTutorial();
    }

    private void StartCrackDoorTututial()
    {
        TaskManager.Instance.StartTask(TaskType.TutorialCrackDoors, 1, -1);
        TaskManager.TaskCompleted += OnCrackedDoor;
        CreateArrow(_crackDoorArrowPoint.position);
        _robWalls.SetActive(true);
    }

    private void OnCrackedDoor(TaskType type)
    {
        if (type != TaskType.TutorialCrackDoors)
            return;
        YandexGame.savesData.TutorialCrackDoorDone = true;
        TaskManager.TaskCompleted -= OnCrackedDoor;
        ClearArrows();
        DOVirtual.DelayedCall(_startingTaskDelay, StartRobHouseTutorial);
    }

    private void StartRobHouseTutorial()
    {
        TaskManager.Instance.StartTask(TaskType.TutorialRobHouse, 3, -1);
        TaskManager.TaskCompleted += OnRobbedHouse;
        foreach (var point in _lootableAreaPoints)
            CreateArrow(point.position);
    }

    private void OnRobbedHouse(TaskType type)
    {
        if (type != TaskType.TutorialRobHouse)
            return;
        YandexGame.savesData.TutorialRobHouseDone = true;
        TaskManager.TaskCompleted -= OnRobbedHouse;
        ClearArrows();
        _robWalls.SetActive(false);
        DOVirtual.DelayedCall(_startingTaskDelay, StartSellItemsTutorial);
    }

    private void StartSellItemsTutorial()
    {
        TaskManager.Instance.StartTask(TaskType.TutorialSellItems, 1, -1);
        TaskManager.TaskCompleted += OnSoldItems;
        CreateArrow(_sellItemsPoint.position);
        _sellWalls.SetActive(true);
    }

    private void OnSoldItems(TaskType type)
    {
        if (type != TaskType.TutorialSellItems)
            return;
        YandexGame.savesData.TutorialSellItemsDone = true;
        TaskManager.TaskCompleted -= OnSoldItems;
        ClearArrows();
        DOVirtual.DelayedCall(_startingTaskDelay, StartBuyUpgradeTutorial);
    }

    private void StartBuyUpgradeTutorial()
    {
        TaskManager.Instance.StartTask(TaskType.TutorialBuyUpgrade, 1, -1);
        TaskManager.TaskCompleted += OnBoughtUpgrade;
        _arrow2d.SetActive(true);
        _arrow2d.transform.position = _upgradePanelButton.position;
    }

    private void OnBoughtUpgrade(TaskType type)
    {
        if (type != TaskType.TutorialBuyUpgrade)
            return;
        YandexGame.savesData.TutorialBuyUpgradeDone = true;
        TaskManager.TaskCompleted -= OnBoughtUpgrade;
        _arrow2d.SetActive(false);
        DOVirtual.DelayedCall(_startingTaskDelay, StartBuyZoneTutorial);
    }

    private void StartBuyZoneTutorial()
    {
        TaskManager.Instance.StartTask(TaskType.TutorialBuyZone, 1, -1);
        TaskManager.TaskCompleted += OnBoughtZone;
        _unlockArea.gameObject.SetActive(true);
        CreateArrow(_unlockArea.position);
    }

    private void OnBoughtZone(TaskType type)
    {
        if (type != TaskType.TutorialBuyZone)
            return;
        YandexGame.savesData.TutorialBuyZoneDone = true;
        YandexGame.savesData.TutorialDone = true;
        TaskManager.TaskCompleted -= OnBoughtZone;
        ClearArrows();
        DOVirtual.DelayedCall(_startingTaskDelay, TaskManager.Instance.StartRandomTask);
    }

    private void CreateArrow(Vector3 position)
    {
        _arrows.Add(Instantiate(_arrow3dPrefab, position, Quaternion.identity, transform).transform);
    }

    private void ClearArrows()
    {
        foreach (var arrow in _arrows)
        {
            if (arrow != null)
                Destroy(arrow.gameObject);
        }
        _arrows.Clear();
    }
}
