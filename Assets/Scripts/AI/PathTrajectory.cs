using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PathTrajectory : MonoBehaviour
{
    [SerializeField] private Transform[] _path;
    [SerializeField] private Transform[] _stopPoints;
    [SerializeField] private bool _loop;
    [SerializeField] private bool _reverse;
    [SerializeField] private float _stoppingDuration = 3.0f;
    [SerializeField] private NavMeshAgent _agent;

    private int _currentIndex;

    private void Start()
    {
        if (TryGetComponent<DrawPath>(out DrawPath path))
            path.Draw(_path, _loop);

        Go();
    }

    public void Go()
    {
        if (_path.Length == 0)
            return;

        if (_path.Length == 1)
        {
            _agent.SetDestination(_path[0].position);
            return;
        }

        _agent.SetDestination(_path[_currentIndex].position);
        _agent.isStopped = false;
    }

    public void Stop()
    {
        _agent.isStopped = true;
    }

    private void FixedUpdate()
    {
        if (_path == null || _path.Length <= 1 || _agent.isStopped)
            return;

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (_stopPoints.Contains(_path[_currentIndex]))
            {
                Stop();
                DOVirtual.DelayedCall(_stoppingDuration, ChangeIndexAndGo);
            }
            else
                ChangeIndexAndGo();
        }
    }

    private void ChangeIndexAndGo()
    {
        _currentIndex += _reverse ? -1 : 1;
        if (_currentIndex == _path.Length || _currentIndex == -1)
        {
            if (_loop)
            {
                _currentIndex = _reverse ? _path.Length - 1 : 0;
            }
            else
            {
                _currentIndex = _reverse ? 1 : _path.Length - 2;
                _reverse = !_reverse;
            }
        }
        Go();
    }
}
