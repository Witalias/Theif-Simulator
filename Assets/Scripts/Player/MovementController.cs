using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PathTrajectory))]
public class MovementController : MonoBehaviour
{
    private const string runAnimatorBool = "Run";
    private const string jumpAnimatorTrigger = "Jump";
    private const string stopJumpAnimatorTrigger = "Stop Jump";
    private const float climbDetectionDistance = 0.1f;

    public static event Action MovingStarted;

    [SerializeField] private float manuallyMovingSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private FloatingJoystick _joystick;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private PathTrajectory _pathTrajectory;
    private WaitingAndAction _waitingAndAction;

    private bool _isMoving = false;
    private bool _controlsLocked;
    private float _initialSpeed;

    public bool Busy { get; set; } = false;

    public Transform CenterPoint { get => centerPoint; }

    public bool IsRunning => _rigidbody.velocity.magnitude > 0;

    public void StopJumpAnimation()
    {
        _animator.SetTrigger(stopJumpAnimatorTrigger);
        Busy = false;
    }

    public void JumpThroughWindow(Vector3 targetPoint)
    {
        if (Busy)
            return;

        Busy = true;
        _animator.SetTrigger(jumpAnimatorTrigger);
        _capsuleCollider.enabled = false;

        void actionAfter()
        {
            Busy = false;
            _animator.SetTrigger(stopJumpAnimatorTrigger);
            _capsuleCollider.enabled = true;
        }

        var direction = new Vector3(targetPoint.x, transform.position.y, targetPoint.z) - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        var path = new Queue<Vector3>(new List<Vector3>
        {
            transform.position,
            targetPoint,
            transform.position + direction * 2.5f
        });
        _pathTrajectory.Go(transform, path, climbSpeed, false, actionAfter, climbDetectionDistance);
    }

    public void AddSpeed(float valueInPercents)
    {
        manuallyMovingSpeed = _initialSpeed + _initialSpeed * valueInPercents / 100f;
    }

    public void RotateTowards(Vector3 point)
    {
        var direction = point - transform.position;
        var angle = Quaternion.LookRotation(direction).eulerAngles;
        transform.DORotate(new Vector3(0f, angle.y, 0f), 0.5f);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _pathTrajectory = GetComponent<PathTrajectory>();

        _initialSpeed = manuallyMovingSpeed;
    }

    private void Start()
    {
        _waitingAndAction = GameObject.FindGameObjectWithTag(Tags.TimeCircle.ToString()).GetComponent<WaitingAndAction>();
        AddSpeed(Stats.Instanse.IncreasedPlayerSpeedInPercents);
    }

    private void OnEnable()
    {
        WaitingAndAction.TimerActived += OnProcessAction;
        UIHoldButton.HoldButtonActived += OnProcessAction;
    }

    private void OnDisable()
    {
        WaitingAndAction.TimerActived -= OnProcessAction;
        UIHoldButton.HoldButtonActived -= OnProcessAction;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (_controlsLocked)
            return;

        var movementVector = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);
        if (movementVector == Vector3.zero)
            movementVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        var direction = new Vector3(movementVector.x, _rigidbody.velocity.y, movementVector.z);
        if (direction.magnitude > 1)
            direction.Normalize();

        _rigidbody.velocity = direction * manuallyMovingSpeed;

        if (movementVector != Vector3.zero)
        {
            AbortSearching();

            _rigidbody.MoveRotation(Quaternion.LookRotation(movementVector));

            if (!_isMoving)
            {
                _isMoving = true;
                _animator.SetBool(runAnimatorBool, true);
                MovingStarted?.Invoke();
            }
        }
        else
        {
            _isMoving = false;
            _animator.SetBool(runAnimatorBool, false);
        }
    }

    private void AbortSearching()
    {
        _waitingAndAction.Abort();
    }

    private void OnProcessAction(bool value)
    {
        _controlsLocked = value;
        _joystick.gameObject.SetActive(!value);
    }
}
