using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Button _upgradePanelButton;
    [SerializeField] private Button _closeUpgradePanelButton;
    [SerializeField] private Transform _buyUpgradeButton;
    [SerializeField] private Transform _unlockArea;

    [Header("Walls")]
    [SerializeField] private GameObject _robWalls;
    [SerializeField] private GameObject _sellWalls;

    private readonly List<Transform> _arrows = new();

    private void Start()
    {
        if (YandexGame.savesData.TutorialDone)
            return;

        _upgradePanelButton.interactable = false;

        if (!YandexGame.savesData.TutorialCrackDoorDone)
            StartCrackDoorTututial();
        else if (!YandexGame.savesData.TutorialRobHouseDone)
            StartRobHouseTutorial();
        else if (!YandexGame.savesData.TutorialSellItemsDone)
            StartSellItemsTutorial();
        else if (!YandexGame.savesData.TutorialBuyUpgradeDone)
            StartBuyUpgradeTutorial();
        else if (!YandexGame.savesData.TutorialBuyZoneDone)
            StartBuyZoneTutorial();
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
        SaveLoad.SaveTutorialCrackDoorDoneBoolean(true);
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
        _robWalls.SetActive(true);
    }

    private void OnRobbedHouse(TaskType type)
    {
        if (type != TaskType.TutorialRobHouse)
            return;
        SaveLoad.SaveTutorialRobHouseDoneBoolean(true);
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
        SaveLoad.SaveTutorialSellItemsDoneBoolean(true);
        TaskManager.TaskCompleted -= OnSoldItems;
        ClearArrows();
        DOVirtual.DelayedCall(_startingTaskDelay, StartBuyUpgradeTutorial);
    }

    private void StartBuyUpgradeTutorial()
    {
        TaskManager.Instance.StartTask(TaskType.TutorialBuyUpgrade, 1, -1);
        TaskManager.TaskCompleted += OnBoughtUpgrade;
        UpgradesPopupButton.Clicked += OnOpenedUpgradesPopup;
        _arrow2d.SetActive(true);
        _arrow2d.transform.position = _upgradePanelButton.transform.position;
        _upgradePanelButton.interactable = true;
        _sellWalls.SetActive(true);
    }

    private void OnOpenedUpgradesPopup()
    {
        UpgradesPopupButton.Clicked -= OnOpenedUpgradesPopup;
        _closeUpgradePanelButton.interactable = false;
        DOVirtual.DelayedCall(0.25f, () => _arrow2d.transform.position = _buyUpgradeButton.position);
    }

    private void OnBoughtUpgrade(TaskType type)
    {
        if (type != TaskType.TutorialBuyUpgrade)
            return;
        TaskManager.TaskCompleted -= OnBoughtUpgrade;
        OpenClosePopup.Opened += OnClosedUpgradesPopup;
        _closeUpgradePanelButton.interactable = true;
        _arrow2d.transform.position = _closeUpgradePanelButton.transform.position;
    }

    private void OnClosedUpgradesPopup(bool opened)
    {
        if (opened)
            return;
        _arrow2d.SetActive(false);
        OpenClosePopup.Opened -= OnClosedUpgradesPopup;
        SaveLoad.SaveTutorialBuyUpgradeDoneBoolean(true);
        DOVirtual.DelayedCall(_startingTaskDelay, StartBuyZoneTutorial);
    }

    private void StartBuyZoneTutorial()
    {
        TaskManager.Instance.StartTask(TaskType.TutorialBuyZone, 1, -1);
        TaskManager.TaskCompleted += OnBoughtZone;
        _unlockArea.gameObject.SetActive(true);
        _sellWalls.SetActive(true);
        CreateArrow(_unlockArea.position);
    }

    private void OnBoughtZone(TaskType type)
    {
        if (type != TaskType.TutorialBuyZone)
            return;
        SaveLoad.SaveTutorialBuyZoneDoneBoolean(true);
        SaveLoad.SaveTutorialDoneBoolean(true);
        TaskManager.TaskCompleted -= OnBoughtZone;
        ClearArrows();
        _upgradePanelButton.interactable = true;
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
