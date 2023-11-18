using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementController : MonoBehaviour
{
    private const string RUN_ANIMATOR_BOOLEAN = "Run";
    private const string CAUGHT_ANIMATOR_TRIGGER = "Caught";

    public static event Action MovingStarted;
    public static event Action PlayerCaught;

    [SerializeField] private bool _controlsLocked;
    [SerializeField] private Stealth _stealth;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private FloatingJoystick _joystick;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private bool _isMoving;
    private bool _canHide;
    private bool _inBuilding;
    private bool _noticed;

    public bool InBuilding => _inBuilding;
    public Transform CenterPoint { get => centerPoint; }
    public bool IsRunning => _isMoving;
    public bool Noticed => _noticed;
    public bool Busy => _controlsLocked;

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
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(delay);
            transform.position = Stats.Instanse.PrisonSpawnPoint.position;
            Stats.Instanse.ClearBackpack();
            _controlsLocked = false;
        }
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
    }

    private void OnEnable()
    {
        WaitingAndAction.TimerActived += OnProcessAction;
        UIHoldButton.HoldButtonActived += OnProcessAction;
        UpgradesPanel.Opened += OnProcessAction;
        Building.PlayerInBuilding += InBuildingState;
        EnemyAI.PlayerIsNoticed += OnNoticed;
    }

    private void OnDisable()
    {
        WaitingAndAction.TimerActived -= OnProcessAction;
        UIHoldButton.HoldButtonActived -= OnProcessAction;
        UpgradesPanel.Opened -= OnProcessAction;
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

        _rigidbody.velocity = direction * Stats.Instanse.PlayerMovingSpeed;

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
        _noticed = true;
    }

    private void InBuildingState(bool value)
    {
        _inBuilding = value;
        CanHide(value);

        if (value == false)
            _noticed = false;
    }
}
