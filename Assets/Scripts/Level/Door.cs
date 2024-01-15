using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour, IIdentifiable
{
    [Serializable]
    public class SavedData
    {
        public int ID;
        public bool IsLocked;
    }

    private const string ANIMATOR_OPEN_BOOLEAN = "Open";
    private const string ANIMATOR_BACK_SIDE_BOOLEAN = "Back Side";

    public static event Action<float, Action, Action, Sound, float> WaitAndExecuteWithSound;
    public static event Action<int> PlayResourceAnimationXp;
    public static event Action BuildingInfoShowed;
    public static event Action Hacked;

    [SerializeField] private float _hackingTime = 10f;
    [SerializeField] private bool _openBackSide;
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
    private readonly SavedData _savedData = new();
    private bool _triggered;
    private bool _isHacking;
    private bool _hacked;

    public int ID { get; set; }

    public SavedData Save()
    {
        _savedData.ID = ID;
        _savedData.IsLocked = !_hacked;
        return _savedData;
    }

    public void Load(SavedData data)
    {
        Lock(data.IsLocked);
    }

    public void HackOrOpen(MovementController player)
    {
        if (_triggered || player == null || player.Busy)
            return;

        if (!_hacked)
        {
            if (!_isHacking && !player.IsRunning && !player.Noticed)
            {
                Hack(player);
                SetActiveBuildingLevelPanel(false);
            }
            _hackingArea.SetActive(!_isHacking);
            return;
        }
        Open();
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
        BuildingInfoShowed?.Invoke();
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

    public void SetBuildingLevel(int level) => _buildingLevelText.text = $"{Translation.GetLevelNameAbbreviated()} {level}";

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
        if (!_buildingLevelPanel.gameObject.activeInHierarchy)
            return;

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

    private void Hack(MovementController player)
    {
        _isHacking = true;
        player.CanHide(false);
        player.RotateTowards(_appearHackingZoneTrigger.transform.position);
        SoundManager.Instanse.PlayLoop(Sound.DoorMasterKey);
        void ActionDone()
        {
            Lock(false);
            player.CanHide(true);
            _isHacking = false;
            var xp = GameSettings.Instanse.HackingXPReward;
            Stats.Instanse.AddXP(xp);
            PlayResourceAnimationXp?.Invoke(xp);
            TaskManager.Instance.ProcessTask(TaskType.HackHouse, 1);
            TaskManager.Instance.ProcessTask(TaskType.TutorialCrackDoors, 1);
            Hacked?.Invoke();
        }
        void ActionAbort()
        {
            player.CanHide(true);
            _isHacking = false;
        }
        WaitAndExecuteWithSound?.Invoke(_hackingTime, ActionDone, ActionAbort, Sound.DoorMasterKey, 0f);
    }

    private void SetState(bool open)
    {
        _triggered = open;
        _collider.enabled = !open;
        _animator.SetBool(ANIMATOR_OPEN_BOOLEAN, open);
        SoundManager.Instanse.PlayOneStream(open ? Sound.DoorOpen : Sound.DoorClose, _audioSource);
    }

    private void SetActiveBuildingLevelPanel(bool value)
    {
        _buildingLevelPanel.DOFade(value ? 0.8f : 0.0f, 0.5f);
    }
}
