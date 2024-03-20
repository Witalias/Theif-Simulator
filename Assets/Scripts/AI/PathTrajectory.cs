using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PathTrajectory : Pathfinder
{
    [SerializeField] private Transform _pathPointsContainer;
    [SerializeField] private StopablePoint[] _stopPoints;
    [SerializeField] private bool _loop;
    [SerializeField] private bool _reverse;
    [SerializeField] protected bool _goOnStart = true;
    
    private Transform[] _path;
    private Tween _currentTween;
    private int _currentIndex;
    protected bool _goNextPointAfterStopping = true;

    protected virtual void Awake()
    {
        _path = _pathPointsContainer.GetComponentsInChildren<Transform>().Skip(1).ToArray();
    }

    protected virtual void Start()
    {
        if (TryGetComponent<DrawPath>(out DrawPath path))
            path.Draw(_path, _loop);

        if (_goOnStart)
            FollowTrajectory();
    }

    protected override void Update()
    {
        if (_path == null || _path.Length == 0)
            return;

        base.Update(); 
    }

    public virtual void FollowTrajectory()
    {
        if (_path.Length == 0)
            return;

        if (_path.Length == 1)
        {
            GoTo(_path[0].position);
            return;
        }

        GoTo(_path[_currentIndex].position);
    }

    protected override void Stop()
    {
        base.Stop();
        _currentTween?.Kill();

        if (!_goNextPointAfterStopping || _path.Length <= 1)
            return;

        var stopPoint = _stopPoints.Where(stopPoint => stopPoint.Point == _path[_currentIndex]);
        if (stopPoint.Count() > 0)
            _currentTween = DOVirtual.DelayedCall(stopPoint.First().Duration, ChangeIndexAndGo);
        else
            ChangeIndexAndGo();
    }

    protected void SetActivePath(bool value) => _pathPointsContainer.gameObject.SetActive(value);

    private void ChangeIndexAndGo()
    {
        if (_reverse)
        {
            if (_currentIndex == 0)
                _reverse = false;
        }
        else if (_currentIndex == _path.Length - 1)
            _reverse = true;
        _currentIndex += _reverse ? -1 : 1;
        FollowTrajectory();
    }

    [Serializable]
    private class StopablePoint
    {
        public Transform Point;
        public float Duration;
    }    
}
