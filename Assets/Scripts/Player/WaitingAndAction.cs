using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Noisy))]
public class WaitingAndAction : MonoBehaviour
{
    private const string showAnimationName = "Show";

    [SerializeField] private Image fill;
    [SerializeField] private float yOffsetFromPlayer = 100f;

    private Coroutine waitingCoroutine;
    private RectTransform rectTransform;
    private Animation anim;
    private Noisy noisy;

    private float reachedTime = 1f;
    private float currentTime = 0f;
    private bool noisyAction = false;
    private float noiseRadius = 0f;
    private Action action;

    public bool InProgress { get; private set; } = false;

    public void WaitAndExecute(float time, Action action, bool noisy = false, float noiseRadius = 0f)
    {
        if (action == null || InProgress)
            return;

        if (time == 0f)
            action.Invoke();

        reachedTime = time;
        this.action = action;
        this.noiseRadius = noiseRadius;
        noisyAction = noisy;
        InProgress = true;

        rectTransform.localPosition = new Vector3(0, yOffsetFromPlayer, 0);
        anim.Play(showAnimationName);

        waitingCoroutine = StartCoroutine(AddTick());
    }

    public void Abort()
    {
        if (!InProgress)
            return;

        StopCoroutine(waitingCoroutine);
        Refresh();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        anim = GetComponent<Animation>();
        noisy = GetComponent<Noisy>();
    }

    private IEnumerator AddTick()
    {
        if (currentTime >= reachedTime)
        {
            action.Invoke();
            Refresh();
            yield break;
        }

        yield return new WaitForSeconds(0.01f);
        currentTime += 0.01f;
        fill.fillAmount = currentTime / reachedTime;

        if (noisyAction)
            noisy.Noise(noiseRadius);

        waitingCoroutine = StartCoroutine(AddTick());
    }

    private void Refresh()
    {
        reachedTime = 1f;
        currentTime = 0f;
        InProgress = false;
        rectTransform.position = new Vector3(-10000, 0, 0);
    }
}
