using UnityEngine;
using UnityEngine.AI;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent _agent;

    protected virtual void Update()
    {
        if (_agent.enabled && !_agent.isStopped && !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            Stop();
    }

    protected virtual void GoTo(Vector3 position)
    {
        if (_agent.enabled)
        {
            _agent.isStopped = false;
            _agent.SetDestination(position);
        }
    }

    protected virtual void GoTo(Transform point)
    {
        GoTo(point.position);
    }

    protected virtual void Stop()
    {
        if (_agent.enabled)
            _agent.isStopped = true;
    }
}
