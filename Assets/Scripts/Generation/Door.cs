using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
    private const string ANIMATOR_OPEN_BOOLEAN = "Open";

    public static event Action<float, Action, Action, Sound, float> WaitAndExecuteWithSound;

    [SerializeField] private float _hackingTime = 10f;
    [SerializeField] private GameObject _hackingArea;
    [SerializeField] private Transform _centerPoint;

    private Animator _animator;
    private AudioSource _audioSource;

    private bool _triggered = false;
    private bool _hacked = false;
    private bool _isHacking = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        MovementController player = null;

        if (!_triggered && (other.GetComponent<EnemyAI>() != null
            || other.TryGetComponent<MovementController>(out player)))
        {
            if (player != null)
            {
                if (!_hacked)
                {
                    if (!_isHacking && !player.IsRunning)
                    {
                        Hack();
                        player.RotateTowards(_centerPoint.position);
                    }
                    _hackingArea.SetActive(!_isHacking);
                    return;
                }
            }
            SetState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggered && (other.GetComponent<MovementController>() != null || other.GetComponent<EnemyAI>() != null))
        {
            SetState(false);
        }
    }

    private void Hack()
    {
        _isHacking = true;
        SoundManager.Instanse.PlayLoop(Sound.DoorMasterKey);
        void ActionDone()
        {
            _hacked = true;
            _isHacking = false;
        }
        void ActionAbort()
        {
            _isHacking = false;
        }
        WaitAndExecuteWithSound?.Invoke(_hackingTime, ActionDone, ActionAbort, Sound.DoorMasterKey, 0f);
    }

    private void SetState(bool open)
    {
        _triggered = open;
        _animator.SetBool(ANIMATOR_OPEN_BOOLEAN, open);
        SoundManager.Instanse.Play(open ? Sound.DoorOpen : Sound.DoorClose, _audioSource);
    }
}
