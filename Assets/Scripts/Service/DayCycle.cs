using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

public class DayCycle : MonoBehaviour
{
    public static DayCycle Instance { get; private set; }

    [SerializeField] private DayCyclePhaseInfo[] _phases;
    [SerializeField] private Light _sun;

    private Dictionary<DayCycleType, DayCyclePhaseInfo> _phasesDict;
    private Tween _currentTween;

    public event Action<DayCycleType> PhaseChanged;

    public DayCycleType CurrentPhase { get; private set; } = DayCycleType.Day;

    private void Awake()
    {
        Instance = this;
        _phasesDict = _phases.ToDictionary(phase => phase.Type);
    }

    private void Start()
    {
        Load();
    }

    public void RunPhase(DayCycleType type)
    {
        ProcessPhase(_phasesDict[type]);
    }

    public void NextPhase()
    {
        ProcessPhase(GetNextPhase(CurrentPhase));
    }

    public void SubscribeOnPhaseChanged(Action<DayCycleType> action) => PhaseChanged += action;

    public void UnsubscribeOnPhaseChanged(Action<DayCycleType> action) => PhaseChanged -= action;

    private void Load()
    {
        var phaseType = DayCycleType.Day;
        if (SaveLoad.HasDayPhaseSave)
            phaseType = Enum.Parse<DayCycleType>(YandexGame.savesData.DayPhase);
        if (YandexGame.savesData.TutorialDone)
            RunPhase(phaseType);
    }

    private void ProcessPhase(DayCyclePhaseInfo phase)
    {
        CurrentPhase = phase.Type;
        SaveLoad.SaveDayPhase(phase.Type);
        _currentTween?.Kill();
        _currentTween = _sun.DOColor(phase.SunColor, phase.TransitionDuration)
            .OnComplete(() =>
            {
                PhaseChanged?.Invoke(phase.Type);
                _currentTween = DOVirtual.DelayedCall(phase.PhaseDuration, () => ProcessPhase(GetNextPhase(phase.Type)));
            });
    }

    private DayCyclePhaseInfo GetNextPhase(DayCycleType type)
    {
        return type switch
        {
            DayCycleType.Day => _phasesDict[DayCycleType.Evening],
            DayCycleType.Evening => _phasesDict[DayCycleType.Night],
            DayCycleType.Night => _phasesDict[DayCycleType.Morning],
            DayCycleType.Morning => _phasesDict[DayCycleType.Day],
            _ => null,
        };
    }
}
