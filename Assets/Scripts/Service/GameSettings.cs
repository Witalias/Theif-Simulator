using UnityEngine;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instanse { get; private set; } = null;

    [SerializeField] private Language language = Language.Russian;
    [SerializeField] private float _appearHackingZonesRadius = 20.0f;
    [SerializeField] private int _stepXPRequirement = 2;
    [SerializeField] private int _maxLevel = 30;
    [SerializeField] private int _hackingXPReward;
    [SerializeField] private int _theftXPReward;

    public Language Language { get => language; set => language = value; }
    public float AppearHackingZonesDistance => _appearHackingZonesRadius;
    public int StepXPRequirement => _stepXPRequirement;
    public int MaxLevel => _maxLevel;
    public int HackingXPReward => _hackingXPReward;
    public int TheftXPReward => _theftXPReward;

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);
    }
}
