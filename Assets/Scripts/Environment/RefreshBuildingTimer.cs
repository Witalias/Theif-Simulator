using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class RefreshBuildingTimer : MonoBehaviour
{
    [SerializeField] private int _secondsForRefresh;

    private int _remainSeconds;
    private Coroutine _tickCoroutine;
    private Action _outAction;

    public void Initialize(Action outAction)
    {
        _outAction = outAction;
    }

    public void StartTimer()
    {
        _tickCoroutine = StartCoroutine(Tick());
    }

    public void StopTimer()
    {
        if (_tickCoroutine != null)
            StopCoroutine(_tickCoroutine);
    }

    private IEnumerator Tick()
    {
        var wait = new WaitForSeconds(1.0f);
        while (_remainSeconds > 0)
        {
            yield return wait;
            _remainSeconds--;
        }
        Refresh();
    }

    private void Refresh()
    {
        _remainSeconds = _secondsForRefresh;
        _outAction?.Invoke();
    }
}
