using UnityEngine;
using System.Collections.Generic;
using YG;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instanse { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool _debugMode;

    [Header("Language")]
    [SerializeField] private bool _loadLanguageFromYG;
    [SerializeField] private Language language = Language.Russian;

    [Header("Properties")]
    [SerializeField] private float _appearHackingZonesRadius = 20.0f;
    [SerializeField] private int _stepXPRequirement = 2;
    [SerializeField] private int _maxLevel = 30;
    [SerializeField] private int _hackingXPReward;
    [SerializeField] private int _theftXPReward;

    public bool DebugMode => _debugMode;
    public Language Language { get => language; set => language = value; }
    public float AppearHackingZonesDistance => _appearHackingZonesRadius;
    public int StepXPRequirement => _stepXPRequirement;
    public int MaxLevel => _maxLevel;
    public int HackingXPReward => _hackingXPReward;
    public int TheftXPReward => _theftXPReward;

    private void Awake()
    {
        Instanse = this;

        if (_loadLanguageFromYG)
            LoadLanguage();
    }

    private void LoadLanguage()
    {
        Language = YandexGame.EnvironmentData.language switch
        {
            "ru" => Language.Russian,
            _ => Language.English,
        };
    }
}
