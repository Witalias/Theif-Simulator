using DG.Tweening;
using System;
using UnityEngine;

public class DogAI : Pathfinder
{
    public static event Action<string, float, bool> GShowQuickMessage;

    [SerializeField] private float _caughtDuration;
    [SerializeField] private VisibilitySlider _visibilityArea;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private ParticleSystem _dustParticle;
    [SerializeField] private MovableAnimatorController _animatorController;

    private Tween _currentTween;
    private MovementController _player;
    private Vector3 _dustPosition;
    private bool _isSleeping = true;
    private bool _isGoingSleeping;
    private bool _lockedControls;

    private void Awake()
    {
        transform.position = _visibilityArea.EntrancePoint.position;
        _dustPosition = _dustParticle.transform.localPosition;
        _mesh.SetActive(false);
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
        if (_player == null)
            _animatorController.WalkBoolean(true);
        else
            _animatorController.RunBoolean(true);
    }

    protected override void Stop()
    {
        base.Stop();
        _animatorController.WalkBoolean(false);
        _animatorController.RunBoolean(false);

        if (_isGoingSleeping)
            Hide();
    }

    private void OnPlayerEnterDoghouseTrigger(MovementController player)
    {
        _player = player;
        if (_isSleeping)
            return;

        _currentTween.Kill();
        _isGoingSleeping = false;
        Stop();
    }

    private void OnPlayerExitDoghouseTrigger(MovementController player)
    {
        _player = null;
        Stop();
        _currentTween.Kill();
        _currentTween = DOVirtual.DelayedCall(2.0f, () =>
        {
            GoSleep();
        });
    }

    private void ShowAndRun()
    {
        _mesh.SetActive(true);
        PlayDustParticle();
        SoundManager.Instanse.Play(Sound.DogBarking);
        GShowQuickMessage?.Invoke($"{Translation.GetAngryDogName()}!", 1.0f, true);
        _lockedControls = false;
        _currentTween = DOVirtual.DelayedCall(0.5f, () => _isSleeping = false);
    }

    private void Hide()
    {
        _isGoingSleeping = false;
        _isSleeping = true;
        _lockedControls = true;
        _mesh.SetActive(false);
        _visibilityArea.SetLockBar(false);
        PlayDustParticle();
    }

    private void GoSleep()
    {
        if (_lockedControls)
            return;

        GoTo(_visibilityArea.EntrancePoint.position);
        _isGoingSleeping = true;
    }

    private bool TryFollow()
    {
        if (_isSleeping || _player == null || _lockedControls)
            return false;

        GoTo(_player.transform.position);
        return true;
    }

    private void Catch(MovementController player)
    {
        if (_isSleeping)
            return;

        Stop();
        player.Caught(_caughtDuration);
        var targetAngle = Quaternion.LookRotation(player.transform.position - transform.position).eulerAngles;
        transform.DORotate(new Vector3(0.0f, targetAngle.y, 0.0f), 0.5f);
        _lockedControls = true;
        DOVirtual.DelayedCall(_caughtDuration, () => _lockedControls = false);
    }

    private void PlayDustParticle()
    {
        _dustParticle.transform.parent = transform;
        _dustParticle.transform.localPosition = _dustPosition;
        _dustParticle.transform.parent = transform.parent;
        _dustParticle.Play();
    }
}
