using DG.Tweening;
using System;
using UnityEngine;

public class DogAI : Pathfinder
{
    public static event Action<string, float, bool> GShowQuickMessage;

    [SerializeField] private float _caughtDuration;
    [SerializeField] private ActivityPhase _activityTime;
    [SerializeField] private VisibilitySlider _visibilityArea;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private ParticleSystem _dustParticle;
    [SerializeField] private GameObject _ZZZParticle;
    [SerializeField] private MovableAnimatorController _animatorController;

    private Tween _currentTween;
    private MovementController _player;
    private Vector3 _dustPosition;
    private bool _isWatching = true;
    private bool _isGoingToHouse;
    private bool _isGoingSleep;
    private bool _isSleeping;
    private bool _lockedControls;

    private bool Followed => _player != null;

    private void Awake()
    {
        transform.position = _visibilityArea.EntrancePoint.position;
        _dustPosition = _dustParticle.transform.localPosition;
        _mesh.SetActive(false);
    }

    private void Start()
    {
        if (_activityTime is not ActivityPhase.Always)
        {
            DayCycle.Instance.SubscribeOnPhaseChanged(CheckActivity);
            CheckActivity(DayCycle.Instance.CurrentPhase);
        }
    }

    private void OnEnable()
    {
        _visibilityArea.SubscribeOnBarFilled(ShowAndRun);
        _visibilityArea.SubscribeOnPlayerEnterTrigger(OnPlayerEnterDoghouseTrigger);
        _visibilityArea.SubscribeOnPlayerExitTrigger(OnPlayerExitDoghouseTrigger);
    }

    private void OnDisable()
    {
        _visibilityArea.UnsubscribeOnBarFilled(ShowAndRun);
        _visibilityArea.UnsubscribeOnPlayerEnterTrigger(OnPlayerEnterDoghouseTrigger);
        _visibilityArea.UnsubscribeOnPlayerExitTrigger(OnPlayerExitDoghouseTrigger);
    }

    protected override void Update()
    {
        TryFollow();
        base.Update();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<MovementController>(out var player))
            return;

        Catch(player);
    }

    protected override void GoTo(Vector3 position)
    {
        base.GoTo(position);
        if (!Followed)
            _animatorController.WalkBoolean(true);
        else
            _animatorController.RunBoolean(true);
    }

    protected override void Stop()
    {
        base.Stop();
        _animatorController.WalkBoolean(false);
        _animatorController.RunBoolean(false);

        if (_isGoingToHouse)
        {
            Hide();
            if (_isGoingSleep)
            {
                _isGoingSleep = false;
                SetSleepState(true);
            }
        }
    }

    private void OnPlayerEnterDoghouseTrigger(MovementController player)
    {
        _player = player;
        if (_isWatching || _isSleeping)
            return;

        _currentTween.Kill();
        _isGoingToHouse = false;
        //_isGoingSleep = false;
        Stop();
    }

    private void OnPlayerExitDoghouseTrigger(MovementController player)
    {
        _player = null;
        if (_isWatching || _isSleeping)
            return;

        Stop();
        _currentTween.Kill();
        _currentTween = DOVirtual.DelayedCall(2.0f, GoToHouse);
    }

    private void ShowAndRun()
    {
        _mesh.SetActive(true);
        PlayDustParticle();
        SoundManager.Instanse.Play(Sound.DogBarking);
        GShowQuickMessage?.Invoke($"{Translation.GetAngryDogName()}!", 1.0f, true);
        _lockedControls = false;
        _currentTween = DOVirtual.DelayedCall(0.5f, () => _isWatching = false);
    }

    private void Hide()
    {
        _isGoingToHouse = false;
        _isWatching = true;
        _lockedControls = true;
        _mesh.SetActive(false);
        PlayDustParticle();

        if (!_isGoingSleep)
            _visibilityArea.SetLockBar(false);
    }

    private void GoToHouse()
    {
        if (_lockedControls)
            return;

        GoTo(_visibilityArea.EntrancePoint.position);
        _isGoingToHouse = true;
    }

    private bool TryFollow()
    {
        if (_isWatching || !Followed || _lockedControls)
            return false;

        GoTo(_player.transform.position);
        return true;
    }

    private void Catch(MovementController player)
    {
        if (_isWatching)
            return;

        Stop();
        player.Caught(_caughtDuration);
        var targetAngle = Quaternion.LookRotation(player.transform.position - transform.position).eulerAngles;
        transform.DORotate(new Vector3(0.0f, targetAngle.y, 0.0f), 0.5f);
        _lockedControls = true;
        DOVirtual.DelayedCall(_caughtDuration, () => _lockedControls = false);
    }

    private void CheckActivity(DayCycleType dayPhase)
    {
        if (_activityTime is ActivityPhase.Always ||
            (dayPhase is DayCycleType.Day && _activityTime is ActivityPhase.Day) ||
            (dayPhase is DayCycleType.Night && _activityTime is ActivityPhase.Night))
        {
            _isGoingSleep = false;
            SetSleepState(false);
        }
        else
        {
            if (_isWatching)
            {
                if (!_isSleeping)
                    SetSleepState(true);
            }
            else
            {
                _isGoingSleep = true;
            }
        }    
    }

    private void SetSleepState(bool value)
    {
        _isSleeping = value;
        _ZZZParticle.SetActive(value);
        _visibilityArea.Refresh();
        _visibilityArea.SetLockBar(value);

        if (value)
            _visibilityArea.SetActiveVisibilityBar(false);
    }

    private void PlayDustParticle()
    {
        _dustParticle.transform.parent = transform;
        _dustParticle.transform.localPosition = _dustPosition;
        _dustParticle.transform.parent = transform.parent;
        _dustParticle.Play();
    }
}
