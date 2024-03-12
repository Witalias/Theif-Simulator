using System;
using System.Collections;
using UnityEngine;

public class CustomTimer : MonoBehaviour
{
    [SerializeField] private float _interval;
    [SerializeField] private GameObject _content;
    [SerializeField] private UIBar _bar;

    private float _currentValue;
    private event Action OnComplete;

    public bool IsOn { get; private set; }

    private void Awake()
    {
        _bar.gameObject.SetActive(false);
    }

    public void SubscribeOnComplete(Action action) => OnComplete += action;

    public void UnsubscribeOnComplete(Action action) => OnComplete -= action;

    public void SetContentActive(bool value) => _content.SetActive(value);

    public void Run()
    {
        if (_interval == 0.0f)
        {
            Complete();
            return;
        }
        _currentValue = 0.0f;
        IsOn = true;
        StopAllCoroutines();
        StartCoroutine(ProccessTimer());
    }

    private IEnumerator ProccessTimer()
    {
        var wait = new WaitForEndOfFrame();
        while (_currentValue < _interval)
        {
            _currentValue += Time.deltaTime;
            _bar.SetValue(_currentValue, _interval);
            yield return wait;
        }
        Complete();
    }

    private void Complete()
    {
        IsOn = false;
        OnComplete?.Invoke();
    }
}
