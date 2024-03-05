using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class TutorialSystem : MonoBehaviour
{
    [SerializeField] private float _startingTaskDelay;
    [SerializeField] private float _switchingCameraDelay;
    [SerializeField] private GameObject _arrow3dPrefab;
    [SerializeField] private GameObject _arrow2d;
    [SerializeField] private MovementController _player;
    [SerializeField] private GameObject _controls;
    [SerializeField] private OpenClosePopup _stealth;
    [SerializeField] private GameObject _unlockArea2;

    [Header("Arrow points")]
    [SerializeField] private Transform _crackDoorArrowPoint;
    [SerializeField] private Transform[] _lootableAreaPoints;
    [SerializeField] private Transform _sellItemsPoint;
    [SerializeField] private Transform _sellButton;
    [SerializeField] private Button _closeMarketPanelButton;
    [SerializeField] private Button _upgradePanelButton;
    [SerializeField] private Button _closeUpgradePanelButton;
    [SerializeField] private Transform _buyUpgradeButton;
    [SerializeField] private Transform _unlockArea;
    [SerializeField] private Transform _safePoint;

    [Header("Walls")]
    [SerializeField] private GameObject _robWalls;
    [SerializeField] private GameObject _sellWalls;

    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineVirtualCamera _doorCamera;
    [SerializeField] private CinemachineVirtualCamera _houseCamera;
    [SerializeField] private CinemachineVirtualCamera _blackMarketCamera;
    [SerializeField] private CinemachineVirtualCamera _unlockAreaCamera;

    private readonly List<Transform> _arrows = new();

    private void Start()
    {
        if (!YandexGame.savesData.TutorialEnterBuildingWithEnemyDone)
            Building.PlayerInBuildingWithEnemyFirstly += OnEnterBuildingWithEnemy;

        if (!YandexGame.savesData.TutorialLootSafeDone)
            Safe.Filled += OnSafeFilled;

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
        _unlockArea2.SetActive(false);
        CameraChanger.Instance.TemporarilySwitchCamera(_doorCamera, _switchingCameraDelay, () =>
        {
            _controls.SetActive(true);
            _player.SubscribeOnMove(OnPlayerMove);
        });
    }

    private void OnPlayerMove(bool value)
    {
        if (value == true)
        {
            _player.UnsubscribeOnMove(OnPlayerMove);
            _controls.SetActive(false);
        }
    }

    private void OnCrackedDoor(TaskType type)
    {
        if (type != TaskType.TutorialCrackDoors)
            return;
        SaveLoad.SaveTutorialCrackDoorDoneBoolean(true);
        TaskManager.TaskCompleted -= OnCrackedDoor;
        MetricaSender.TutorialStep("Взломать дверь");
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
        _unlockArea2.SetActive(false);
        CameraChanger.Instance.TemporarilySwitchCamera(_houseCamera, _switchingCameraDelay);
    }

    private void OnRobbedHouse(TaskType type)
    {
        if (type != TaskType.TutorialRobHouse)
            return;
        SaveLoad.SaveTutorialRobHouseDoneBoolean(true);
        TaskManager.TaskCompleted -= OnRobbedHouse;
        MetricaSender.TutorialStep("Ограбить дом");
        ClearArrows();
        _robWalls.SetActive(false);
        DOVirtual.DelayedCall(_startingTaskDelay, StartSellItemsTutorial);
    }

    private void StartSellItemsTutorial()
    {
        TaskManager.Instance.StartTask(TaskType.TutorialSellItems, 1, -1);
        TaskManager.TaskCompleted += OnSoldItems;
        BlackMarketArea.PlayerStayed += OnOpenedMarketPopup;
        _sellWalls.SetActive(true);
        _unlockArea2.SetActive(false);
        CameraChanger.Instance.TemporarilySwitchCamera(_blackMarketCamera, _switchingCameraDelay);
        CreateArrow(_sellItemsPoint.position);
    }

    private void OnOpenedMarketPopup()
    {
        BlackMarketArea.PlayerStayed -= OnOpenedMarketPopup;
        _closeMarketPanelButton.interactable = false;
        DOVirtual.DelayedCall(0.25f, () =>
        {
            _arrow2d.SetActive(true);
            _arrow2d.transform.position = _sellButton.position;
        });
    }

    private void OnSoldItems(TaskType type)
    {
        if (type != TaskType.TutorialSellItems)
            return;
        TaskManager.TaskCompleted -= OnSoldItems;
        OpenClosePopup.Opened += OnClosedMarketPanel;
        MetricaSender.TutorialStep("Продать предметы");
        _closeMarketPanelButton.interactable = true;
        _arrow2d.transform.position = _closeMarketPanelButton.transform.position;
        ClearArrows();
    }

    private void OnClosedMarketPanel(bool opened)
    {
        if (opened)
            return;
        OpenClosePopup.Opened -= OnClosedMarketPanel;
        SaveLoad.SaveTutorialSellItemsDoneBoolean(true);
        _arrow2d.SetActive(false);
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
        _unlockArea2.SetActive(false);
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
        MetricaSender.TutorialStep("Купить улучшение");
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
        _unlockArea2.SetActive(false);
        CreateArrow(_unlockArea.position);
        CameraChanger.Instance.TemporarilySwitchCamera(_unlockAreaCamera, _switchingCameraDelay);
    }

    private void OnBoughtZone(TaskType type)
    {
        if (type != TaskType.TutorialBuyZone)
            return;
        SaveLoad.SaveTutorialBuyZoneDoneBoolean(true);
        SaveLoad.SaveTutorialDoneBoolean(true);
        TaskManager.TaskCompleted -= OnBoughtZone;
        MetricaSender.TutorialStep("Открыть зону");
        ClearArrows();
        _upgradePanelButton.interactable = true;
        _unlockArea2.SetActive(true);
        DOVirtual.DelayedCall(_startingTaskDelay, TaskManager.Instance.StartRandomTask);
    }

    private void OnEnterBuildingWithEnemy()
    {
        Building.PlayerInBuildingWithEnemyFirstly -= OnEnterBuildingWithEnemy;
        SaveLoad.SaveTutorialEnterBuildingWithEnemyDoneBoolean(true);
        MetricaSender.TutorialStep("Окно со стелсом");
        _stealth.Open();
    }

    private void OnSafeFilled()
    {
        Safe.Filled -= OnSafeFilled;
        SaveLoad.SaveTutorialLootSafeDoneBoolean(true);
        CreateArrow(_safePoint.position);
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
