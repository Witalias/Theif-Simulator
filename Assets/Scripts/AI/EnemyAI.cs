using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DG.Tweening;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(FieldOfView))]
public class EnemyAI : MonoBehaviour
{
    private const string WALK_ANIMATOR_BOOLEAN = "Walk";
    private const string RUN_ANIAMTOR_BOOLEAN = "Run";
    private const string REACT_TO_NOISE_ANIMATOR_TRIGGER = "React To Noise";
    private const string SCARY_ANIMATOR_TRIGGER = "Scary";

    public static event Action PlayerIsNoticed;
    public static event Action<string, float> ShowQuickMessage;

    [SerializeField] private bool _isWoman;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _caughtDuration;
    [SerializeField] private Color _detectViewColor;
    [SerializeField] private PathTrajectory _pathTrajectory;
    [SerializeField] private SpriteRenderer _view;

    private Animator _animator;
    private NavMeshAgent _agent;
    private FieldOfView _vision;
    private MovementController _player;
    private AudioSource _audioSource;
    private Color _defaultViewColor;
    private Building _building;
    private Tween _detectTween;
    private float _defaultSpeed;
    private bool _worried;
    private bool _followed;
    private bool _lockedControls;

    public bool Worried => _worried;

    public void Initialize(Building building)
    {
        _building = building;
    }

    public void Calm()
    {
        if (!_worried)
            return;

        Stop();
        _detectTween.Kill();
        _followed = false;
        _worried = false;
        _agent.speed = _defaultSpeed;
        _view.color = _defaultViewColor;
        _animator.SetTrigger(SCARY_ANIMATOR_TRIGGER);
        PlayNotFindSound();
        DOVirtual.DelayedCall(2.0f, () =>
        {
            if (!_worried)
                Patrol();
        });
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<FieldOfView>();
        _audioSource = GetComponent<AudioSource>();
        _defaultViewColor = _view.color;
        _defaultSpeed = _agent.speed;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        Patrol();
    }

    private void Update()
    {
        if (_lockedControls)
            return;

        Detect();
        Follow();
        //SearchTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_followed || !collision.gameObject.TryGetComponent<MovementController>(out MovementController player))
            return;

        Stop();
        player.Caught(_caughtDuration);
        if (_building != null)
            _building.LockDoors(true);
        _lockedControls = true;
        DOVirtual.DelayedCall(_caughtDuration + Time.deltaTime, () => _lockedControls = false);
    }

    private void Patrol()
    {
        _pathTrajectory.Go();
        Run();
    }

    private void Run()
    {
        _agent.isStopped = false;

        if (_worried)
            _animator.SetBool(RUN_ANIAMTOR_BOOLEAN, true);
        else
            _animator.SetBool(WALK_ANIMATOR_BOOLEAN, true);
    }

    private void Stop()
    {
        _animator.SetBool(RUN_ANIAMTOR_BOOLEAN, false);
        _animator.SetBool(WALK_ANIMATOR_BOOLEAN, false);
        _pathTrajectory.Stop();
    }

    private void Detect()
    {
        if (!_vision.canSeePlayer || _worried || !_player.InBuilding || _player.Hided)
            return;

        Stop();
        _worried = true;
        //_inSearching = false;
        _agent.speed = _followSpeed;
        _animator.SetTrigger(REACT_TO_NOISE_ANIMATOR_TRIGGER);
        _view.color = _detectViewColor;
        PlayScreamSound();
        PlayerIsNoticed?.Invoke();
        ShowQuickMessage?.Invoke("NOTICED!", 1.0f);
        _detectTween = DOVirtual.DelayedCall(1.0f, () =>
        {
            _followed = true;
            Run();
        });
    }

    private void Follow()
    {
        if (!_followed)
            return;

        _agent.SetDestination(_player.transform.position);
    }

    //private void SearchTarget()
    //{
    //    if (!_followed || _inSearching)
    //        return;

    //    _inSearching = true;
    //    if (_searchTargetCoroutine != null)
    //        StopCoroutine(_searchTargetCoroutine);
    //    _searchTargetCoroutine = StartCoroutine(Coroutine());

    //    IEnumerator Coroutine()
    //    {
    //        yield return new WaitForSeconds(_searchingDuration);
    //        Calm();
    //    }
    //}

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
