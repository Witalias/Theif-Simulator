using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PathTrajectory : Pathfinder
{
    [SerializeField] private Transform _pathPointsContainer;
    [SerializeField] private Transform[] _stopPoints;
    [SerializeField] private bool _loop;
    [SerializeField] private bool _reverse;
    [SerializeField] private bool _goOnStart = true;
    [SerializeField] private float _stoppingDuration = 3.0f;
    
    private Transform[] _path;
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
        if (_path == null || _path.Length <= 1)
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

        if (!_goNextPointAfterStopping)
            return;

        if (_stopPoints.Contains(_path[_currentIndex]))
            DOVirtual.DelayedCall(_stoppingDuration, ChangeIndexAndGo);
        else
            ChangeIndexAndGo();
    }

    private void ChangeIndexAndGo()
    {
        if (_reverse)
        {
            if (_currentIndex == 0)
                _reverse = false;
            else
                _currentIndex--;
        }
        else
        {
            if (_currentIndex == _path.Length - 1)
                _reverse = true;
            else
                _currentIndex++;
        }
        //_currentIndex += _reverse ? -1 : 1;
        //if (_currentIndex == _path.Length || _currentIndex == -1)
        //{
        //    if (_loop)
        //    {
        //        _currentIndex = _reverse ? _path.Length - 1 : 0;
        //    }
        //    else
        //    {
        //        _currentIndex = _reverse ? 1 : _path.Length - 2;
        //        _reverse = !_reverse;
        //    }
        //}
        FollowTrajectory();
    }
}
