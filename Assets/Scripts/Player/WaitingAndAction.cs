using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Noisy))]
public class WaitingAndAction : MonoBehaviour
{
    private const string ANIMATOR_SHOW_TRIGGER = "Show";
    private const string ANIMATOR_PULSATE_TRIGGER = "Pulsate";

    public static event Action<bool> TimerActived;

    [SerializeField] private float _tapTimePercents = 10f;
    [SerializeField] private Image fill;
    [SerializeField] private float yOffsetFromPlayer = 100f;

    private Coroutine waitingCoroutine;
    private RectTransform rectTransform;
    private Animator _animator;
    private Noisy noisy;

    private float reachedTime = 1f;
    private float currentTime = 0f;
    private bool noisyAction = false;
    private bool playSound = false;
    private float noiseRadius = 0f;
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
    
    public void WaitAndExecute(float reachedTime, Action actionDone, Action actionAbort, Sound sound, float currentTime = 0f)
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
        currentTime += (reachedTime - currentTime) * percents / 100f;
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
        noisy = GetComponent<Noisy>();
    }

    private void Update()
    {
        if (!InProgress)
            return;

        if (Input.GetMouseButtonDown(0))
            AddProgress(_tapTimePercents);
    }

    private IEnumerator ProcessTicks()
    {
        var wait = new WaitForEndOfFrame();
        while (currentTime < reachedTime)
        {
            yield return wait;
            currentTime += Time.deltaTime;
            fill.fillAmount = currentTime / reachedTime;

            if (noisyAction)
                noisy.Noise(noiseRadius);
        }
        _actionDone.Invoke();
        Refresh();
    }

    private void Refresh()
    {
        //reachedTime = 1f;
        //currentTime = 0f;
        noiseRadius = 0f;
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
