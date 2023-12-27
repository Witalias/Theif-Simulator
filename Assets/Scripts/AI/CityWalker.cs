using UnityEngine;
using UnityEngine.AI;

public class CityWalker : MonoBehaviour
{
    private const string WALK_ANIMATOR_BOOL = "Walk";

    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    private bool _isWalk;

    private void Update()
    {
        if (!_agent.isStopped && !_isWalk)
        {
            _animator.SetBool(WALK_ANIMATOR_BOOL, true);
            _isWalk = true;
        }
        else if (_agent.isStopped)
        {
            _animator.SetBool(WALK_ANIMATOR_BOOL, false);
            _isWalk = false;
        }
    }
}
