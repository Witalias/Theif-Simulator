using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Building[] _buildings;

    private void Start()
    {
        Load();
        Initialize();
    }

    private void OnEnable()
    {
        Building.StatsChanged += Save;
    }

    private void OnDisable()
    {
        Building.StatsChanged -= Save;
    }

    private void Save()
    {
        if (YandexGame.savesData.TutorialRobHouseDone)
            SaveLoad.SaveBuildings(_buildings.Select(building => building.Save()));
    }

    private void Load()
    {
        if (SaveLoad.HasBuildingsSave)
        {
            var loadedBuildings = SaveLoad.LoadBuildings();
            foreach (var building in _buildings)
                building.Load(loadedBuildings[building.GetHashCode()]);
        }
    }

    private void Initialize()
    {
        foreach (var building in _buildings)
            building.Initialize();
    }
}
