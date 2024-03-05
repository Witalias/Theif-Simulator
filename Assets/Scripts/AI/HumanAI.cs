using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DG.Tweening;
using System;

public class HumanAI : PathTrajectory
{
    public static event Action PlayerIsNoticed;
    public static event Action<string, float, bool> ShowQuickMessage;

    [SerializeField] private bool _isWoman;
    [SerializeField] private bool _calmOnTimer;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _caughtDuration;
    [SerializeField] private Color _detectViewColor;
    [SerializeField] private SpriteRenderer _view;
    [SerializeField] private HumanAnimatorController _animatorController;
    [SerializeField] private FieldOfView _vision;
    [SerializeField] private AudioSource _audioSource;

    private MovementController _player;
    private Color _defaultViewColor;
    private Building _building;
    private Tween _detectTween;
    private float _defaultSpeed;
    private bool _worried;
    private bool _followed;
    private bool _lockedControls;

    public bool Worried => _worried;

    protected override void Awake()
    {
        base.Awake();
        _defaultViewColor = _view.color;
        _defaultSpeed = _agent.speed;
    }

    protected override void Start()
    {
        base.Start();
        _player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

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
        _view.color = _defaultViewColor;
        _animatorController.ScaryTrigger();
        PlayNotFindSound();
        DOVirtual.DelayedCall(2.0f, () =>
        {
            if (!_worried)
                FollowTrajectory();
        });
    }

    protected override void GoTo(Vector3 position)
    {
        base.GoTo(position);
        if (_worried)
            _animatorController.RunBoolean(true);
        else
            _animatorController.WalkBoolean(true);
    }

    protected override void Stop()
    {
        _animatorController.RunBoolean(false);
        _animatorController.WalkBoolean(false);
        base.Stop();
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
        if (!_vision.CanSeePlayer || _worried || !_player.InBuilding || _player.Hided)
            return;

        _goNextPointAfterStopping = false;
        Stop();
        _worried = true;
        _agent.speed = _followSpeed;
        _animatorController.ScaryTrigger();
        _view.color = _detectViewColor;
        PlayScreamSound();
        PlayerIsNoticed?.Invoke();
        ShowQuickMessage?.Invoke($"{Translation.GetNoticedName()}!", 1.0f, true);
        _detectTween = DOVirtual.DelayedCall(1.0f, () => _followed = true);
    }

    private void TryFollow()
    {
        if (!_followed)
            return;

        GoTo(_player.transform.position);
    }

    private void PlaySuspectSound()
    {
        if (_isWoman)
            SoundManager.Instanse.Play(Sound.SuspectWoman, _audioSource);
        else
            SoundManager.Instanse.Play(Sound.SuspectMan, _audioSource);
    }

    private void PlayNotFindSound()
    {
        if (_isWoman)
            SoundManager.Instanse.Play(Sound.NotFindWoman, _audioSource);
        else
            SoundManager.Instanse.Play(Sound.NotFindMan, _audioSource);
    }

    private void PlayScreamSound()
    {
        if (_isWoman)
            SoundManager.Instanse.Play(Sound.ScreamWoman, _audioSource);
        else
            SoundManager.Instanse.Play(Sound.ScreamMan, _audioSource);
    }
}
