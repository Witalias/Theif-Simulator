using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Animator))]
public class WaitingAndAction : MonoBehaviour
{
    private const string ANIMATOR_SHOW_TRIGGER = "Show";
    private const string ANIMATOR_PULSATE_TRIGGER = "Pulsate";

    public static event Action<bool> TimerActived;

    [SerializeField] private Color _negativeColor;
    [SerializeField] private Color _positiveColor;
    [SerializeField] private GameObject _content;
    [SerializeField] private Image _fill;
    [SerializeField] private GameObject _taskPanel;

    private Coroutine _waitingCoroutine;
    private Animator _animator;
    private float _reachedTime = 1.0f;
    private float _currentTime = 0.0f;
    private bool _playSound = false;
    private Sound _sound;
    private Action _actionDone;
    private Action _actionAbort;

    public bool InProgress { get; private set; } = false;

    public void WaitAndExecute(float reachedTime, Action actionDone, Action actionAbort, float currentTime = 0f)
    {
        if (actionDone == null || InProgress)
            return;

        if (reachedTime <= currentTime)
        {
            actionDone.Invoke();
            return;
        }

        _reachedTime = reachedTime;
        _currentTime = currentTime;
        _actionDone = actionDone;
        _actionAbort = actionAbort;
        InProgress = true;

        _content.SetActive(true);
        _animator.SetTrigger(ANIMATOR_SHOW_TRIGGER);

        _waitingCoroutine = StartCoroutine(ProcessTicks());
        TimerActived?.Invoke(true);
        _taskPanel.SetActive(false);
    }
    
    public void WaitAndExecuteWithSound(float reachedTime, Action actionDone, Action actionAbort, Sound sound, float currentTime = 0f)
    {
        _sound = sound;
        _playSound = true;
        WaitAndExecute(reachedTime, actionDone, actionAbort, currentTime);
    }

    public void AddProgress(float percents)
    {
        percents = Mathf.Clamp(percents, 0f, 100f);
        _currentTime += _reachedTime * percents / 100f;
        _currentTime = Mathf.Clamp(_currentTime, 0f, _reachedTime);
        _animator.SetTrigger(ANIMATOR_PULSATE_TRIGGER);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Door.WaitAndExecuteWithSound += WaitAndExecuteWithSound;
        HumanAI.PlayerIsNoticed += Abort;
        MovementController.PlayerCaught += Abort;
        OpenClosePopup.Opened += OnOpenPopup;
    }

    private void OnDisable()
    {
        Door.WaitAndExecuteWithSound -= WaitAndExecuteWithSound;
        HumanAI.PlayerIsNoticed -= Abort;
        MovementController.PlayerCaught -= Abort;
        OpenClosePopup.Opened -= OnOpenPopup;
    }

    private void Update()
    {
        if (!InProgress)
            return;

        if (Input.GetMouseButtonDown(0))
            AddProgress(GameData.Instanse.TapBonusTimePercents);
    }

    private IEnumerator ProcessTicks()
    {
        var wait = new WaitForEndOfFrame();
        while (_currentTime < _reachedTime)
        {
            yield return wait;
            _currentTime += Time.deltaTime;
            _fill.fillAmount = _currentTime / _reachedTime;
            _fill.color = Color.Lerp(_negativeColor, _positiveColor, _fill.fillAmount);
        }
        _actionDone.Invoke();
        Refresh();
    }

    private void Refresh()
    {
        _taskPanel.SetActive(true);
        InProgress = false;

        if (_playSound)
        {
            _playSound = false;
            SoundManager.Instanse.Stop(_sound);
        }
        _content.SetActive(false);
        TimerActived?.Invoke(false);
    }

    private void Abort()
    {
        if (!InProgress)
            return;

        _actionAbort?.Invoke();
        StopCoroutine(_waitingCoroutine);
        Refresh();
    }

    private void OnOpenPopup(bool arg)
    {
        Abort();
    }
}
