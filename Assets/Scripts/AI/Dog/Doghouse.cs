using System;
using System.Collections;
using UnityEngine;

public class Doghouse : MonoBehaviour
{
    public static event Action GPlayerIsNoticed;
    public static event Action<string, float, bool> ShowQuickMessage;

    [SerializeField] private float _increaseSpeed;
    [SerializeField] private float _decreaseSpeed;
    [SerializeField] private UIBar _visibilityBar;
    [SerializeField] private Animation _visibilityAnimation;
    [SerializeField] private Transform _entrancePoint;

    private event Action OnVisibilityBarFilled;
    private event Action<MovementController> OnPlayerEnterTrigger;
    private event Action<MovementController> OnPlayerExitTrigger;
    private float _currentValue;
    private bool _lockedBar;

    public Vector3 EntrancePosition => _entrancePoint.position;

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

        if (_lockedBar)
            DetectPlayer();

        if (player.IsMoving)
            StartCoroutine(ProccessIncreaseVisibility());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<MovementController>(out var player))
            return;

        player.UnsubscribeOnMove(OnPlayerMove);
        OnPlayerExitTrigger?.Invoke(player);
        StopAllCoroutines();
        StartCoroutine(ProccessDecreaseVisibility());
    }

    public void SubscribeOnBarFilled(Action action) => OnVisibilityBarFilled += action;

    public void UnsubscribeOnBarFilled(Action action) => OnVisibilityBarFilled -= action;

    public void SubscribeOnPlayerEnterTrigger(Action<MovementController> action) => OnPlayerEnterTrigger += action;

    public void UnsubscribeOnPlayerEnterTrigger(Action<MovementController> action) => OnPlayerEnterTrigger -= action;

    public void SubscribeOnPlayerExitTrigger(Action<MovementController> action) => OnPlayerExitTrigger += action;

    public void UnsubscribeOnPlayerExitTrigger(Action<MovementController> action) => OnPlayerExitTrigger -= action;

    public void SetLockBar(bool value) => _lockedBar = value;

    private void OnPlayerMove(bool value)
    {
        StopAllCoroutines();
        if (value == true)
            StartCoroutine(ProccessIncreaseVisibility());
        else
            StartCoroutine(ProccessDecreaseVisibility());
    }

    private IEnumerator ProccessIncreaseVisibility()
    {
        if (_currentValue >= 1.0f)
            yield break;

        _visibilityBar.gameObject.SetActive(true);
        _visibilityAnimation.Play();
        var wait = new WaitForEndOfFrame();
        while (_currentValue <= 1.0f)
        {
            if (_lockedBar)
            {
                yield return new WaitForSeconds(2.0f);
                continue;
            }
            _currentValue += Time.deltaTime * _increaseSpeed;
            _visibilityBar.SetValue(_currentValue, 1.0f);
            yield return wait;
        }
        OnVisibilityBarFilled?.Invoke();
        DetectPlayer();
        SetLockBar(true);
    }

    private IEnumerator ProccessDecreaseVisibility()
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
            _currentValue -= Time.deltaTime * _decreaseSpeed;
            _visibilityBar.SetValue(_currentValue, 1.0f);
            yield return wait;
        }
        _visibilityBar.gameObject.SetActive(false);
    }

    private void DetectPlayer()
    {
        GPlayerIsNoticed?.Invoke();
        ShowQuickMessage?.Invoke($"{Translation.GetAngryDogName()}!", 1.0f, true);
    }
}
