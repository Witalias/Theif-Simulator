using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CreatureVision))]
[RequireComponent(typeof(AudioSource))]
public class EnemyAI : MonoBehaviour
{
    private const string WALK_ANIMATOR_BOOLEAN = "Walk";
    private const string REACT_TO_NOISE_ANIMATOR_TRIGGER = "React To Noise";
    private const string SCARY_ANIMATOR_TRIGGER = "Scary";

    [SerializeField] private bool _isWoman;
    [SerializeField] private PathTrajectory _pathTrajectory;

    private Animator _animator;
    private NavMeshAgent _agent;
    private CreatureVision _vision;
    private MovementController _player;
    private AudioSource _audioSource;

    private bool _worried = false;
    private bool _isPatrolling = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<CreatureVision>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        Patrol();
    }

    private void Patrol()
    {
        _pathTrajectory.Go();
        Run();
    }

    private void Run()
    {
        _animator.SetBool(WALK_ANIMATOR_BOOLEAN, true);
    }

    private void Stop()
    {
        _animator.SetBool(WALK_ANIMATOR_BOOLEAN, false);
        _pathTrajectory.Stop();
    }

    private void PlaySuspectSound()
    {
        if (_isWoman)
            SoundManager.Instanse.Play(Sound.SuspectWoman, _audioSource);
        else
            SoundManager.Instanse.Play(Sound.SuspectMan, _audioSource);
    }

    private void PlayNotFindSound()
    {
        if (_isWoman)
            SoundManager.Instanse.Play(Sound.NotFindWoman, _audioSource);
        else
            SoundManager.Instanse.Play(Sound.NotFindMan, _audioSource);
    }

    private void PlayScreamSound()
    {
        if (_isWoman)
            SoundManager.Instanse.Play(Sound.ScreamWoman, _audioSource);
        else
            SoundManager.Instanse.Play(Sound.ScreamMan, _audioSource);
    }
}
