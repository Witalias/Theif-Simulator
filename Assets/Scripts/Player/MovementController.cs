using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;
using System;
using System.Collections;
using YG;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementController : MonoBehaviour
{
    private const string RUN_ANIMATOR_BOOLEAN = "Run";
    private const string SNEAK_ANIMATOR_BOOLEAN = "Sneak";
    private const string HACK_ANIMATOR_BOOLEAN = "Hack";
    private const string CAUGHT_ANIMATOR_BOOLEAN = "Sit";

    public static event Action MovingStarted;
    public static event Action PlayerCaught;

    [SerializeField] private bool _controlsLocked;
    [SerializeField] private Stealth _stealth;
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private PlayerParticles _particles;
    [SerializeField] private UnityEvent _onCaught;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private Building _currentBuilding;
    private bool _isMoving;
    private bool _canHide;
    private bool _noticed;

    public bool InBuilding => _currentBuilding != null;
    public bool IsRunning => _isMoving;
    public bool Noticed => _noticed;
    public bool Busy => _controlsLocked;
    public bool Hided => _stealth.Hided;

    public void RotateTowards(Vector3 point)
    {
        var direction = point - transform.position;
        var angle = Quaternion.LookRotation(direction).eulerAngles;
        transform.DORotate(new Vector3(0f, angle.y, 0f), 0.5f);
    }

    public void Caught(float delay)
    {
        PlayerCaught?.Invoke();
        _onCaught?.Invoke();
        _controlsLocked = true;
        _animator.SetBool(CAUGHT_ANIMATOR_BOOLEAN, true);
        _particles.ActivateSmokeParticles(true);
        _noticed = false;
        //InBuildingState(false, null);
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(delay);
            transform.position = GameData.Instanse.PrisonSpawnPoint.position;
            SavePosition();
            GameData.Instanse.Backpack.ClearBackpack();
            _controlsLocked = false;
            CanHide(true);
            _animator.SetBool(CAUGHT_ANIMATOR_BOOLEAN, false);
            _particles.ActivateSmokeParticles(false);
        }
    }

    public void CanHide(bool value)
    {
        if (value == true && (!InBuilding || !_currentBuilding.ContainsEnemies()))
            return;

        _canHide = value;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        LoadPosition();
    }

    private void OnEnable()
    {
        WaitingAndAction.TimerActived += OnHack;
        UIHoldButton.HoldButtonActived += OnHack;
        OpenClosePopup.OpenedLate += OnProcessAction;
        Building.PlayerInBuilding += InBuildingState;
        EnemyAI.PlayerIsNoticed += OnNoticed;
        Door.BuildingInfoShowed += SavePosition;
        BlackMarketArea.PlayerExit += SavePosition;
        LevelManager.PlayerInBuilding += GetInBuildingBoolean;
        LevelManager.NewUnlockAreasIsShowing += OnProcessAction;
    }

    private void OnDisable()
    {
        WaitingAndAction.TimerActived -= OnHack;
        UIHoldButton.HoldButtonActived -= OnHack;
        OpenClosePopup.OpenedLate -= OnProcessAction;
        Building.PlayerInBuilding -= InBuildingState;
        EnemyAI.PlayerIsNoticed -= OnNoticed;
        Door.BuildingInfoShowed -= SavePosition;
        BlackMarketArea.PlayerExit -= SavePosition;
        LevelManager.PlayerInBuilding -= GetInBuildingBoolean;
        LevelManager.NewUnlockAreasIsShowing -= OnProcessAction;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private bool GetInBuildingBoolean() => InBuilding;

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

        _rigidbody.velocity = direction * GameData.Instanse.PlayerMovingSpeed;

        if (movementVector != Vector3.zero)
        {
            _rigidbody.MoveRotation(Quaternion.LookRotation(movementVector));

            if (!_isMoving)
            {
                _isMoving = true;
                _animator.SetBool(RUN_ANIMATOR_BOOLEAN, true);
                MovingStarted?.Invoke();

                DOVirtual.DelayedCall(Time.deltaTime, () => {
                    if (_canHide)
                    {
                        _stealth.Show();
                        //_particles.ActivateFastSmokeParticle();
                    }
                });
                
            }
        }
        else if (_isMoving)
        {
            Stop();
        }
    }

    private void Stop()
    {
        _isMoving = false;
        _animator.SetBool(RUN_ANIMATOR_BOOLEAN, false);

        DOVirtual.DelayedCall(Time.deltaTime, () =>
        {
            if (_canHide)
            {
                _stealth.Hide();
                //_particles.ActivateFastSmokeParticle();
            }
        });
    }

    private void OnHack(bool value)
    {
        OnProcessAction(value);
        _animator.SetBool(HACK_ANIMATOR_BOOLEAN, value);
    }

    private void OnProcessAction(bool value)
    {
        _controlsLocked = value;
        _joystick.OnPointerUp(null);
        _joystick.gameObject.SetActive(!value);
        if (value == true)
            Stop();
    }

    private void OnNoticed()
    {
        _animator.SetBool(SNEAK_ANIMATOR_BOOLEAN, false);
        CanHide(false);
        _noticed = true;
    }

    private void InBuildingState(bool inBuilding, Building building)
    {
        _animator.SetBool(SNEAK_ANIMATOR_BOOLEAN, inBuilding);
        if (inBuilding)
        {
            _currentBuilding = building;
            CanHide(building.ContainsEnemies());
            CameraChanger.Instance.SwitchToIndoorCamera();
        }
        else
        {
            _noticed = false;
            _currentBuilding = null;
            CanHide(false);
            CameraChanger.Instance.SwitchToMainCamera();
        }
    }

    private void SavePosition()
    {
        SaveLoad.SavePlayerPosition(transform.position);
    }

    private void LoadPosition()
    {
        if (SaveLoad.HasPlayerPositionSave && YandexGame.savesData.TutorialDone)
            transform.position = SaveLoad.LoadPlayerPosition();
        else
            transform.position = GameData.Instanse.InitialPlayerSpawnPoint.position;
    }
}
