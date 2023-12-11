using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
    private const string ANIMATOR_OPEN_BOOLEAN = "Open";
    private const string ANIMATOR_BACK_SIDE_BOOLEAN = "Back Side";

    public static event Action<float, Action, Action, Sound, float> WaitAndExecuteWithSound;
    public static event Action<int> PlayResourceAnimationXp;

    [SerializeField] private float _hackingTime = 10f;
    [SerializeField] private bool _openBackSide;
    [SerializeField] private Vector2 _minMaxXP;
    [SerializeField] private GameObject _hackingArea;
    [SerializeField] private GameObject _appearHackingZoneTrigger;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private TMP_Text _buildingLevelText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private UIBar _progressBar;
    [SerializeField] private GameObject _barIcon;
    [SerializeField] private CanvasGroup _buildingLevelPanel;

    private Animator _animator;
    private AudioSource _audioSource;

    private bool _triggered;
    private bool _hacked;
    private bool _isHacking;

    public void HackOrOpen(MovementController player)
    {
        if (_triggered || player == null || player.Busy)
            return;

        if (!_hacked)
        {
            if (!_isHacking && !player.IsRunning)
            {
                Hack();
                player.RotateTowards(_appearHackingZoneTrigger.transform.position);
                SetActiveBuildingLevelPanel(false);
            }
            _hackingArea.SetActive(!_isHacking);
            return;
        }
        SetState(true);
    }

    public void Close()
    {
        if (!_triggered)
            return;

        SetState(false);
    }

    public void Open()
    {
        if (_triggered)
            return;

        SetState(true);
    }

    public void ShowBuildingInfo()
    {
        SetActiveBuildingLevelPanel(true);
    }

    public void HideBuildingInfo()
    {
        SetActiveBuildingLevelPanel(false);
    }

    public void Lock(bool value)
    {
        _hacked = !value;
        _appearHackingZoneTrigger.SetActive(value);
    }

    public void SetBuildingLevel(int level) => _buildingLevelText.text = $"LVL {level}";

    public void SetTimerText(int seconds)
    {
        if (!_timerText.gameObject.activeSelf)
            _timerText.gameObject.SetActive(true);

        _timerText.text = TimeSpan.FromSeconds(seconds).ToString("mm\\:ss");

        if (seconds <= 0)
            DOVirtual.DelayedCall(1.0f, () => _timerText.gameObject.SetActive(false));
    }

    public void SetProgressBarValue(int value, int maxValue, string text = null)
    {
        _progressBar.SetValue(value, maxValue);
        _progressBar.SetText(text);
        _barIcon.SetActive(text == null);
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _animator.SetBool(ANIMATOR_BACK_SIDE_BOOLEAN, _openBackSide);
        _buildingLevelPanel.alpha = 0.0f;
    }

    private void Hack()
    {
        _isHacking = true;
        SoundManager.Instanse.PlayLoop(Sound.DoorMasterKey);
        void ActionDone()
        {
            Lock(false);
            _isHacking = false;
            var xp = Randomizator.GetRandomValue(_minMaxXP);
            Stats.Instanse.AddXP(xp);
            PlayResourceAnimationXp?.Invoke(xp);
            TaskManager.Instance.ProcessTask(TaskType.HackHouse, 1);
        }
        void ActionAbort()
        {
            _isHacking = false;
        }
        WaitAndExecuteWithSound?.Invoke(_hackingTime, ActionDone, ActionAbort, Sound.DoorMasterKey, 0f);
    }

    private void SetState(bool open)
    {
        _triggered = open;
        _collider.enabled = !open;
        _animator.SetBool(ANIMATOR_OPEN_BOOLEAN, open);
        SoundManager.Instanse.Play(open ? Sound.DoorOpen : Sound.DoorClose, _audioSource);
    }

    private void SetActiveBuildingLevelPanel(bool value)
    {
        _buildingLevelPanel.DOFade(value ? 0.8f : 0.0f, 0.5f);
    }
}
