using System;
using System.Collections;
using UnityEngine;

public class VisibilitySlider : MonoBehaviour
{
    public static event Action GPlayerIsNoticed;

    [SerializeField] private float _increaseSpeed;
    [SerializeField] private float _decreaseSpeed;
    [SerializeField] private UIBar _visibilityBar;
    [SerializeField] private Animation _visibilityAnimation;
    [SerializeField] private Transform _entrancePoint;
    [SerializeField] private Transform _locationPoint;

    private event Action OnVisibilityBarFilled;
    private event Action<MovementController> OnPlayerEnterTrigger;
    private event Action<MovementController> OnPlayerExitTrigger;
    private float _currentValue;
    private bool _lockedBar;

    public Transform EntrancePoint => _entrancePoint;
    public Transform LocationPoint => _locationPoint;

    private void Awake()
    {
        _visibilityBar.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<MovementController>(out var player))
            return;

        player.SubscribeOnMove(OnPlayerMove);
        OnPlayerEnterTrigger?.Invoke(player);
        StopAllCoroutines();

        //if (_lockedBar)
        //    DetectPlayer();

        if (player.IsMoving)
            StartCoroutine(ProcessIncreaseVisibility());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<MovementController>(out var player))
            return;

        player.UnsubscribeOnMove(OnPlayerMove);
        player.NotNotice();
        OnPlayerExitTrigger?.Invoke(player);
        StopAllCoroutines();
        StartCoroutine(ProcessDecreaseVisibility());
    }

    public void SubscribeOnBarFilled(Action action) => OnVisibilityBarFilled += action;

    public void UnsubscribeOnBarFilled(Action action) => OnVisibilityBarFilled -= action;

    public void SubscribeOnPlayerEnterTrigger(Action<MovementController> action) => OnPlayerEnterTrigger += action;

    public void UnsubscribeOnPlayerEnterTrigger(Action<MovementController> action) => OnPlayerEnterTrigger -= action;

    public void SubscribeOnPlayerExitTrigger(Action<MovementController> action) => OnPlayerExitTrigger += action;

    public void UnsubscribeOnPlayerExitTrigger(Action<MovementController> action) => OnPlayerExitTrigger -= action;

    public void SetVisibilityBar(bool value) => _visibilityBar.gameObject.SetActive(value);

    public void SetLockBar(bool value) => _lockedBar = value;

    public void Refresh()
    {
        SetValue(0.0f);
        SetLockBar(false);
    }

    private void OnPlayerMove(bool value)
    {
        StopAllCoroutines();
        if (gameObject.activeInHierarchy)
        {
            if (value == true)
                StartCoroutine(ProcessIncreaseVisibility());
            else
                StartCoroutine(ProcessDecreaseVisibility());
        }
    }

    private IEnumerator ProcessIncreaseVisibility()
    {
        if (_currentValue >= 1.0f)
            yield break;

        if (!_lockedBar)
        {
            _visibilityBar.gameObject.SetActive(true);
            _visibilityAnimation.Play();
        }
        var wait = new WaitForEndOfFrame();
        while (_currentValue <= 1.0f)
        {
            if (_lockedBar)
            {
                yield return new WaitForSeconds(2.0f);
                continue;
            }
            SetValue(_currentValue + Time.deltaTime * _increaseSpeed);
            yield return wait;
        }
        OnVisibilityBarFilled?.Invoke();
        DetectPlayer();
        SetLockBar(true);
    }

    private IEnumerator ProcessDecreaseVisibility()
    {
        if (_currentValue <= 0.0f)
            yield break;

        //_visibilityAnimation.Stop();
        var wait = new WaitForEndOfFrame();
        while (_currentValue > 0)
        {
            if (_lockedBar)
            {
                yield return new WaitForSeconds(2.0f);
                continue;
            }
            SetValue(_currentValue - Time.deltaTime * _decreaseSpeed);
            yield return wait;
        }
        _visibilityBar.gameObject.SetActive(false);
    }

    private void SetValue(float value)
    {
        _currentValue = value;
        _visibilityBar.SetValue(value, 1.0f);
    }

    private void DetectPlayer()
    {
        GPlayerIsNoticed?.Invoke();
    }
}
