using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Animator))]
public class WaitingAndAction : MonoBehaviour
{
    private const string ANIMATOR_SHOW_TRIGGER = "Show";
    private const string ANIMATOR_PULSATE_TRIGGER = "Pulsate";

    public static event Action<bool> TimerActived;

    [SerializeField] private Color _negativeColor;
    [SerializeField] private Color _positiveColor;
    [SerializeField] private Image fill;
    [SerializeField] private float yOffsetFromPlayer = 100f;

    private Coroutine waitingCoroutine;
    private RectTransform rectTransform;
    private Animator _animator;

    private float reachedTime = 1f;
    private float currentTime = 0f;
    private bool playSound = false;
    private Sound sound;
    private Action _actionDone;
    private Action _actionAbort;

    public bool InProgress { get; private set; } = false;

    public float CurrentTime => currentTime;

    public float ReachedTime => reachedTime;

    public void WaitAndExecute(float reachedTime, Action actionDone, Action actionAbort, float currentTime = 0f)
    {
        if (actionDone == null || InProgress)
            return;

        if (reachedTime <= currentTime)
        {
            actionDone.Invoke();
            return;
        }

        this.reachedTime = reachedTime;
        this.currentTime = currentTime;
        _actionDone = actionDone;
        _actionAbort = actionAbort;
        InProgress = true;

        rectTransform.localPosition = new Vector3(0, yOffsetFromPlayer, 0);
        _animator.SetTrigger(ANIMATOR_SHOW_TRIGGER);

        waitingCoroutine = StartCoroutine(ProcessTicks());
        TimerActived?.Invoke(true);
    }

    //public void WaitAndExecute(float reachedTime, Action action, float noiseRadius)
    //{
    //    this.noiseRadius = noiseRadius;
    //    noisyAction = true;
    //    WaitAndExecute(reachedTime, action);
    //}
    
    public void WaitAndExecuteWithSound(float reachedTime, Action actionDone, Action actionAbort, Sound sound, float currentTime = 0f)
    {
        this.sound = sound;
        this.playSound = true;
        WaitAndExecute(reachedTime, actionDone, actionAbort, currentTime);
    }

    //public void WaitAndExecute(float time, Action actionDone, Action actionAbort, Sound sound, float noiseRadius)
    //{
    //    this.sound = sound;
    //    this.playSound = true;
    //    this.noiseRadius = noiseRadius;
    //    noisyAction = true;
    //    WaitAndExecute(time, actionDone, actionAbort);
    //}

    public void AddProgress(float percents)
    {
        percents = Mathf.Clamp(percents, 0f, 100f);
        currentTime += reachedTime * percents / 100f;
        currentTime = Mathf.Clamp(currentTime, 0f, reachedTime);
        _animator.SetTrigger(ANIMATOR_PULSATE_TRIGGER);
    }

    public void Abort()
    {
        if (!InProgress)
            return;

        _actionAbort?.Invoke();
        StopCoroutine(waitingCoroutine);
        Refresh();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Door.WaitAndExecuteWithSound += WaitAndExecuteWithSound;
    }

    private void OnDisable()
    {
        Door.WaitAndExecuteWithSound -= WaitAndExecuteWithSound;
    }

    private void Update()
    {
        if (!InProgress)
            return;

        if (Input.GetMouseButtonDown(0))
            AddProgress(GameSettings.Instanse.TapBonusTimePercents);
    }

    private IEnumerator ProcessTicks()
    {
        var wait = new WaitForEndOfFrame();
        while (currentTime < reachedTime)
        {
            yield return wait;
            currentTime += Time.deltaTime;
            fill.fillAmount = currentTime / reachedTime;
            fill.color = Color.Lerp(_negativeColor, _positiveColor, fill.fillAmount);
        }
        _actionDone.Invoke();
        Refresh();
    }

    private void Refresh()
    {
        //reachedTime = 1f;
        //currentTime = 0f;
        InProgress = false;

        if (playSound)
        {
            playSound = false;
            SoundManager.Instanse.Stop(sound);
        }
        rectTransform.position = new Vector3(-10000, 0, 0);
        TimerActived?.Invoke(false);
    }
}
