using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DG.Tweening;
using System;

public class HumanAI : PathTrajectory
{
    public static event Action GPlayerIsNoticed;
    public static event Action<string, float, bool> ShowQuickMessage;

    [SerializeField] private bool _isWoman;
    [SerializeField] private bool _alwaysRun;
    [SerializeField] private bool _calmOnTimer;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _caughtDuration;
    [SerializeField] private ActivityPhase _activityTime;
    [SerializeField] private VisibilitySlider _visibilityArea;
    [SerializeField] private Material _detectViewMaterial;
    [SerializeField] private DrawFieldOfView _visionCone;
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private HumanAnimatorController _animatorController;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _ZZZParticle;

    private MovementController _player;
    private Material _defaultViewMaterial;
    private Building _building;
    private Tween _detectTween;
    private float _defaultSpeed;
    private bool _worried;
    private bool _followed;
    private bool _lockedControls;
    private bool _isGoingSleeping;
    private bool _isSleeping;

    public bool Worried => _worried;

    protected override void Awake()
    {
        base.Awake();
        _defaultSpeed = _agent.speed;
    }

    protected override void Start()
    {
        base.Start();
        _defaultViewMaterial = _visionCone.Material;
        _player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();

        if (_visibilityArea != null)
        {
            _visibilityArea.SetLockBar(true);
            if (_activityTime != ActivityPhase.Always)
            {
                DayCycle.Instance.SubscribeOnPhaseChanged(CheckActivity);
                _visibilityArea.SubscribeOnBarFilled(WakeUpAndDetect);
                CheckActivity(DayCycle.Instance.CurrentPhase);
            }
        }
    }

    //private void OnEnable()
    //{
    //    if (_activityTime != ActivityPhase.Always && _visibilityArea != null)
    //    {
    //        DayCycle.Instance.SubscribeOnPhaseChanged(CheckActivity);
    //        _visibilityArea.SubscribeOnBarFilled(WakeUpAndDetect);
    //    }
    //}

    //private void OnDisable()
    //{
    //    if (_activityTime != ActivityPhase.Always && _visibilityArea != null)
    //    {
    //        DayCycle.Instance.UnsubscribeOnPhaseChanged(CheckActivity);
    //        _visibilityArea.UnsubscribeOnBarFilled(WakeUpAndDetect);
    //    }
    //}

    protected override void Update()
    {
        base.Update();

        if (_lockedControls)
            return;

        TryDetect();
        TryFollow();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_followed || !collision.gameObject.TryGetComponent<MovementController>(out MovementController player))
            return;

        Catch(player);
    }

    public void Initialize(Building building)
    {
        _building = building;
    }

    public void Calm()
    {
        if (!_worried || _lockedControls)
            return;

        Stop();
        _detectTween.Kill();
        _goNextPointAfterStopping = true;
        _followed = false;
        _worried = false;
        _agent.speed = _defaultSpeed;
        _visionCone.SetMaterial(_defaultViewMaterial);
        _animatorController.ScaryTrigger();
        PlayNotFindSound();
        DOVirtual.DelayedCall(2.0f, () =>
        {
            if (!_worried)
                CheckActivity(DayCycle.Instance.CurrentPhase);
        });
    }

    protected override void GoTo(Vector3 position)
    {
        base.GoTo(position);
        if (_worried || _alwaysRun)
            _animatorController.RunBoolean(true);
        else
            _animatorController.WalkBoolean(true);
    }

    protected override void Stop()
    {
        _animatorController.RunBoolean(false);
        _animatorController.WalkBoolean(false);

        base.Stop();

        if (_isGoingSleeping)
        {
            _isGoingSleeping = false;
            SetSleepState(true);
        }
    }

    private void Catch(MovementController player)
    {
        Stop();
        player.Caught(_caughtDuration);
        if (_building != null)
            _building.LockDoors(true);
        _lockedControls = true;
        _animatorController.TakeBoolean(true);
        DOVirtual.DelayedCall(_caughtDuration + Time.deltaTime, () =>
        {
            _lockedControls = false;
            _animatorController.TakeBoolean(false);
            Calm();
        });
    }

    private void TryDetect()
    {
        if (!_fieldOfView.CanSeePlayer || _worried || !_player.InBuilding || _player.Hided)
            return;

        Detect();
    }

    private void Detect()
    {
        _goNextPointAfterStopping = false;
        _isGoingSleeping = false;
        Stop();
        _worried = true;
        _agent.speed = _followSpeed;
        _animatorController.ScaryTrigger();
        _visionCone.SetMaterial(_detectViewMaterial);
        PlayScreamSound();
        GPlayerIsNoticed?.Invoke();
        ShowQuickMessage?.Invoke($"{Translation.GetNoticedName()}!", 1.0f, true);
        _detectTween = DOVirtual.DelayedCall(1.0f, () => _followed = true);
    }

    private void TryFollow()
    {
        if (!_followed)
            return;

        GoTo(_player.transform.position);
    }

    private void CheckActivity(DayCycleType dayPhase)
    {
        if (_activityTime == ActivityPhase.Always || _visibilityArea == null ||
            (dayPhase == DayCycleType.Day && _activityTime == ActivityPhase.Day) ||
            (dayPhase == DayCycleType.Night && _activityTime == ActivityPhase.Night))
        {
            if (_isSleeping)
                SetSleepState(false);
            _isGoingSleeping = false;
            FollowTrajectory();
            SetActivePath(true);
        }
        else if (!_isSleeping)
        {
            GoSleep();
        }
    }

    private void SetSleepState(bool value)
    {
        _isSleeping = value;
        _goNextPointAfterStopping = !value;
        _fieldOfView.SetEnable(!value);
        _visionCone.gameObject.SetActive(!value);
        _ZZZParticle.SetActive(value);
        _visibilityArea.Refresh();
        _visibilityArea.SetLockBar(!value);

        if (value)
        {
            _agent.enabled = false;
            _animatorController.SleepTrigger();
            AudioManager.Instanse.PlayLoop(AudioType.Snore, _audioSource);
        }
        else
        {
            _animatorController.StopSleepTrigger();
            _visibilityArea.SetActiveVisibilityBar(false);
            AudioManager.Instanse.Stop(AudioType.Snore);
        }

        transform.DORotate(value ? _visibilityArea.LocationPoint.localEulerAngles : new Vector3(0.0f, transform.rotation.y, 0.0f), 0.25f);
        transform.DOMove(value ? _visibilityArea.LocationPoint.position : _visibilityArea.EntrancePoint.position, 0.25f)
            .OnComplete(() =>
            {
                if (!value)
                    _agent.enabled = true;
            });
    }

    private void GoSleep()
    {
        _goNextPointAfterStopping = false;
        _goOnStart = false;
        SetActivePath(false);
        Stop();
        _isGoingSleeping = true;
        GoTo(_visibilityArea.EntrancePoint.position);
    }

    private void WakeUpAndDetect()
    {
        SetSleepState(false);
        Detect();
    }

    private void PlaySuspectSound()
    {
        if (_isWoman)
            AudioManager.Instanse.Play(AudioType.SuspectWoman, _audioSource);
        else
            AudioManager.Instanse.Play(AudioType.SuspectMan, _audioSource);
    }

    private void PlayNotFindSound()
    {
        if (_isWoman)
            AudioManager.Instanse.Play(AudioType.NotFindWoman, _audioSource);
        else
            AudioManager.Instanse.Play(AudioType.NotFindMan, _audioSource);
    }

    private void PlayScreamSound()
    {
        if (_isWoman)
            AudioManager.Instanse.Play(AudioType.ScreamWoman, _audioSource);
        else
            AudioManager.Instanse.Play(AudioType.ScreamMan, _audioSource);
    }
}
