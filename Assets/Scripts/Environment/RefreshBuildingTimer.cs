using System;
using System.Collections;
using UnityEngine;
using YG;

[RequireComponent(typeof(Building))]
public class RefreshBuildingTimer : MonoBehaviour
{
    [SerializeField] private int _secondsForRefresh;
    [SerializeField] private bool _enabled = true;

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
        if (_enabled)
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
        while (!YandexGame.savesData.TutorialDone)
        {
            yield return wait;
        }
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
