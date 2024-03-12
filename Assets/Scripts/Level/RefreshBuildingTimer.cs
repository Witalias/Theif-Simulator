using System;
using System.Collections;
using UnityEngine;
using YG;

public class RefreshBuildingTimer : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;
    [SerializeField] private int _secondsForRefresh = 120;

    private Coroutine _tickCoroutine;
    private Action _outAction;
    private Action<int> _tickAction;

    public int RemainSeconds { get; set; }

    public void Initialize(Action outAction, Action<int> tickAction)
    {
        _outAction = outAction;
        _tickAction = tickAction;
        RemainSeconds = _secondsForRefresh + 1;
    }

    public void StartTimer()
    {
        if (_enabled)
            _tickCoroutine = StartCoroutine(Tick());
    }

    public void StopTimer()
    {
        if (_tickCoroutine != null)
        {
            StopCoroutine(_tickCoroutine);
            _tickCoroutine = null;
        }
    }

    private IEnumerator Tick()
    {
        var wait = new WaitForSeconds(1.0f);
        while (!YandexGame.savesData.TutorialDone)
        {
            yield return wait;
        }
        while (RemainSeconds > 0)
        {
            RemainSeconds--;
            _tickAction?.Invoke(RemainSeconds);
            yield return wait;
        }
        Refresh();
    }

    private void Refresh()
    {
        RemainSeconds = _secondsForRefresh + 1;
        _outAction?.Invoke();
        _tickCoroutine = null;
    }
}
