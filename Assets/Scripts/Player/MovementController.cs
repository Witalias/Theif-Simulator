using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;
using System.Collections;
using YG;

public class MovementController : MonoBehaviour
{
    public static event Action GPlayerCaught;

    [SerializeField] private bool _controlsLocked;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Stealth _stealth;
    [SerializeField] private HumanAnimatorController _animatorController;
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private PlayerParticles _particles;
    [SerializeField] private UnityEvent _onCaught;

    private event Action<bool> OnMove;
    private Building _currentBuilding;
    private bool _isMoving;
    private bool _canHide;
    private bool _noticed;

    public bool InBuilding => _currentBuilding != null;
    public bool IsRunning => _isMoving;
    public bool Noticed => _noticed;
    public bool Busy => _controlsLocked;
    public bool Hided => _stealth.Hided;
    public bool IsMoving => _isMoving;

    private void Start()
    {
        LoadPosition();
    }

    private void OnEnable()
    {
        TapTapAction.GTimerActived += OnHack;
        UIHoldButton.GHoldButtonActived += OnHack;
        OpenClosePopup.GOpenedLate += OnProcessAction;
        Building.GPlayerInBuilding += InBuildingState;
        HumanAI.GPlayerIsNoticed += Notice;
        VisibilitySlider.GPlayerIsNoticed += Notice;
        Door.GBuildingInfoShowed += SavePosition;
        BlackMarketArea.GPlayerExit += SavePosition;
        LevelManager.GPlayerInBuilding += GetInBuildingBoolean;
        LevelManager.GNewUnlockAreasIsShowing += OnProcessAction;
        Cheats.GBuildingUpgraded += UpgrageCurrentBuildingDebug;
    }

    private void OnDisable()
    {
        TapTapAction.GTimerActived -= OnHack;
        UIHoldButton.GHoldButtonActived -= OnHack;
        OpenClosePopup.GOpenedLate -= OnProcessAction;
        Building.GPlayerInBuilding -= InBuildingState;
        HumanAI.GPlayerIsNoticed -= Notice;
        VisibilitySlider.GPlayerIsNoticed -= Notice;
        Door.GBuildingInfoShowed -= SavePosition;
        BlackMarketArea.GPlayerExit -= SavePosition;
        LevelManager.GPlayerInBuilding -= GetInBuildingBoolean;
        LevelManager.GNewUnlockAreasIsShowing -= OnProcessAction;
        Cheats.GBuildingUpgraded -= UpgrageCurrentBuildingDebug;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void RotateTowards(Vector3 point)
    {
        var direction = point - transform.position;
        var angle = Quaternion.LookRotation(direction).eulerAngles;
        transform.DORotate(new Vector3(0f, angle.y, 0f), 0.5f);
    }

    public void Caught(float delay)
    {
        GPlayerCaught?.Invoke();
        _onCaught?.Invoke();
        _controlsLocked = true;
        _animatorController.SitBoolean(true);
        _particles.ActivateSmokeParticles(true);
        _noticed = false;
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(delay);
            transform.position = GameData.Instanse.PrisonSpawnPoint.position;
            SavePosition();
            GameData.Instanse.Backpack.ClearBackpack();
            _controlsLocked = false;
            CanHide(true);
            _animatorController.SitBoolean(false);
            _particles.ActivateSmokeParticles(false);
        }
    }

    public void CanHide(bool value)
    {
        if (value == true && (!InBuilding || !_currentBuilding.ContainsEnemies()))
            return;

        _canHide = value;
    }

    public void Notice()
    {
        SetNotice(true);
    }

    public void NotNotice()
    {
        if (_currentBuilding != null && !_currentBuilding.ContainsWorriedEnemies())
            SetNotice(false);
    }

    public void SubscribeOnMove(Action<bool> action) => OnMove += action;

    public void UnsubscribeOnMove(Action<bool> action) => OnMove -= action;

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
                _animatorController.RunBoolean(true);
                OnMove?.Invoke(true);

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
        _animatorController.RunBoolean(false);
        OnMove?.Invoke(false);

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
        _animatorController.TakeBoolean(value);
    }

    private void OnProcessAction(bool value)
    {
        _controlsLocked = value;
        _joystick.OnPointerUp(null);
        _joystick.gameObject.SetActive(!value);
        if (value == true)
            Stop();
    }

    private void SetNotice(bool value)
    {
        _animatorController.SneakBoolean(!value);
        CanHide(!value);
        _noticed = value;
    }

    private void InBuildingState(bool inBuilding, Building building)
    {
        _animatorController.SneakBoolean(inBuilding);
        if (inBuilding)
        {
            _currentBuilding = building;
            CanHide(building.ContainsEnemies());
            if (building.IndoorCameraEnabled)
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

    private void UpgrageCurrentBuildingDebug()
    {
        if (_currentBuilding != null)
            _currentBuilding.NextLevelDebug();
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
