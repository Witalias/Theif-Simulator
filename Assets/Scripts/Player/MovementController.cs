using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementController : MonoBehaviour
{
    private const string RUN_ANIMATOR_BOOLEAN = "Run";
    private const string CAUGHT_ANIMATOR_TRIGGER = "Caught";

    public static event Action MovingStarted;
    public static event Action PlayerCaught;

    [SerializeField] private float manuallyMovingSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private Stealth _stealth;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private FloatingJoystick _joystick;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private bool _isMoving;
    private bool _controlsLocked;
    private bool _canHide;
    private bool _inBuilding;
    private float _initialSpeed;

    public bool InBuilding => _inBuilding;

    public Transform CenterPoint { get => centerPoint; }

    public bool IsRunning => _isMoving;

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

    public void Caught(float delay)
    {
        PlayerCaught?.Invoke();
        _controlsLocked = true;
        _animator.SetTrigger(CAUGHT_ANIMATOR_TRIGGER);
        InBuildingState(false);
        DOVirtual.DelayedCall(delay, () =>
        {
            transform.position = Stats.Instanse.PrisonSpawnPoint.position;
            _controlsLocked = false;
        });
    }

    public void CanHide(bool value)
    {
        if (value == true && !_inBuilding)
            return;

        _canHide = value;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        _initialSpeed = manuallyMovingSpeed;
    }

    private void OnEnable()
    {
        WaitingAndAction.TimerActived += OnProcessAction;
        UIHoldButton.HoldButtonActived += OnProcessAction;
        Building.PlayerInBuilding += InBuildingState;
        EnemyAI.PlayerIsNoticed += OnNoticed;
    }

    private void OnDisable()
    {
        WaitingAndAction.TimerActived -= OnProcessAction;
        UIHoldButton.HoldButtonActived -= OnProcessAction;
        Building.PlayerInBuilding -= InBuildingState;
        EnemyAI.PlayerIsNoticed -= OnNoticed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (_controlsLocked)
            return;

        var movementVector = new Vector3(_joystick.Horizontal, 0.0f, _joystick.Vertical);
        if (movementVector == Vector3.zero)
            movementVector = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

        var direction = new Vector3(movementVector.x, 0.0f, movementVector.z);
        if (direction.magnitude > 1)
            direction.Normalize();

        _rigidbody.velocity = direction * manuallyMovingSpeed;

        if (movementVector != Vector3.zero)
        {
            _rigidbody.MoveRotation(Quaternion.LookRotation(movementVector));

            if (!_isMoving)
            {
                _isMoving = true;
                _animator.SetBool(RUN_ANIMATOR_BOOLEAN, true);
                MovingStarted?.Invoke();

                if (_canHide)
                    _stealth.Show();
            }
        }
        else if (_isMoving)
        {
            _isMoving = false;
            _animator.SetBool(RUN_ANIMATOR_BOOLEAN, false);

            if (_canHide)
                _stealth.Hide();
        }
    }

    private void OnProcessAction(bool value)
    {
        _controlsLocked = value;
        _joystick.gameObject.SetActive(!value);
    }

    private void OnNoticed()
    {
        CanHide(false);
    }

    private void InBuildingState(bool value)
    {
        _inBuilding = value;
        CanHide(value);
    }
}
