using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

public class LevelManager : MonoBehaviour
{
    public static event Func<bool> PlayerInBuilding;
    public static event Action<bool> NewUnlockAreasIsShowing;

    [SerializeField] private Building[] _buildings;
    [SerializeField] private UnlockArea[] _unlockAreas;

    private readonly List<CinemachineVirtualCamera> _newAvailableAreaCameras = new();
    private Coroutine _moveCameraToPointsCoroutine;
    private bool _cameraIsMoving;

    private void Start()
    {
        Load();
        Initialize();
    }

    private void OnEnable()
    {
        Building.StatsChanged += SaveBuildings;
        UnlockArea.CostChanged += SaveUnlockAreas;
        UnlockArea.MoveCamera += MoveCameraToNewAvailableArea;
        Building.PlayerInBuilding += PlayerInBuildingState;
    }

    private void OnDisable()
    {
        Building.StatsChanged -= SaveBuildings;
        UnlockArea.CostChanged -= SaveUnlockAreas;
        UnlockArea.MoveCamera -= MoveCameraToNewAvailableArea;
        Building.PlayerInBuilding -= PlayerInBuildingState;
    }

    private void SaveBuildings()
    {
        if (YandexGame.savesData.TutorialRobHouseDone)
            SaveLoad.SaveBuildings(_buildings.Select(building => building.Save()));        
    }

    private void SaveUnlockAreas()
    {
        SaveLoad.SaveUnlockAreas(_unlockAreas.Select(area => area.Save()));
    }

    private void Load()
    {
        if (SaveLoad.HasBuildingsSave)
        {
            var loadedBuildings = SaveLoad.LoadBuildings();
            foreach (var building in _buildings)
            {
                var hash = building.GetHashCode();
                if (loadedBuildings.ContainsKey(hash))
                    building.Load(loadedBuildings[hash]);
            }    
        }
        if (SaveLoad.HasUnlockAreasSave)
        {
            var loadedUnlockAreas = SaveLoad.LoadUnlockAreas();
            foreach (var unlockArea in _unlockAreas)
            {
                var hash = unlockArea.GetHashCode();
                if (loadedUnlockAreas.ContainsKey(hash))
                    unlockArea.Load(loadedUnlockAreas[hash]);
            }
        }
    }

    private void Initialize()
    {
        foreach (var building in _buildings)
            building.Initialize();
    }

    private void MoveCameraToNewAvailableArea(CinemachineVirtualCamera toCamera)
    {
        if (!YandexGame.savesData.TutorialDone)
            return;

        _newAvailableAreaCameras.Add(toCamera);
        var playerInBuilding = PlayerInBuilding?.Invoke();
        if (!playerInBuilding.GetValueOrDefault())
        {
            StopAllCoroutines();
            _moveCameraToPointsCoroutine = StartCoroutine(ProcessMoveCameraToPoints(_newAvailableAreaCameras));
        }          
    }

    private void PlayerInBuildingState(bool value, Building arg)
    {
        if (value == false && _moveCameraToPointsCoroutine == null && _newAvailableAreaCameras.Count > 0 && YandexGame.savesData.TutorialDone)
            _moveCameraToPointsCoroutine = StartCoroutine(ProcessMoveCameraToPoints(_newAvailableAreaCameras));
    }

    private IEnumerator ProcessMoveCameraToPoints(IEnumerable<CinemachineVirtualCamera> toCameras)
    {
        var delay = 3.0f;
        var wait = new WaitForSeconds(delay);
        NewUnlockAreasIsShowing?.Invoke(true);
        foreach (var camera in _newAvailableAreaCameras)
        {
            CameraChanger.Instance.TemporarilySwitchCamera(camera, delay);
            yield return wait;
        }
        _newAvailableAreaCameras.Clear();
        _moveCameraToPointsCoroutine = null;
        NewUnlockAreasIsShowing?.Invoke(false);
    }
}
