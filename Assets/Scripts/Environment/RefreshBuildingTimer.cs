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
    private Action<int> _tickAction;

    public void Initialize(Action outAction, Action<int> tickAction)
    {
        _outAction = outAction;
        _tickAction = tickAction;
        _remainSeconds = _secondsForRefresh + 1;
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
            _remainSeconds--;
            _tickAction?.Invoke(_remainSeconds);
            yield return wait;
        }
        Refresh();
    }

    private void Refresh()
    {
        _remainSeconds = _secondsForRefresh + 1;
        _outAction?.Invoke();
    }
}
