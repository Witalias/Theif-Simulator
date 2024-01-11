using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Building[] _buildings;
    [SerializeField] private UnlockArea[] _unlockAreas;

    private void Start()
    {
        Load();
        Initialize();
    }

    private void OnEnable()
    {
        Building.StatsChanged += SaveBuildings;
        UnlockArea.CostChanged += SaveUnlockAreas;
    }

    private void OnDisable()
    {
        Building.StatsChanged -= SaveBuildings;
        UnlockArea.CostChanged -= SaveUnlockAreas;
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
}
