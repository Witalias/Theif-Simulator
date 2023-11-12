using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
    private const string ANIMATOR_OPEN_BOOLEAN = "Open";

    public static event Action<float, Action, Action, Sound, float> WaitAndExecuteWithSound;
    public static event Action<ResourceType, int, int> PlayResourceAnimation;

    [SerializeField] private float _hackingTime = 10f;
    [SerializeField] private Vector2 _minMaxXP;
    [SerializeField] private GameObject _hackingArea;
    [SerializeField] private GameObject _appearHackingZoneTrigger;

    private Animator _animator;
    private AudioSource _audioSource;

    private bool _triggered = false;
    private bool _hacked = false;
    private bool _isHacking = false;

    public void HackOrOpen(MovementController player)
    {
        if (!_triggered && player != null)
        {
            if (!_hacked)
            {
                if (!_isHacking && !player.IsRunning)
                {
                    Hack();
                    player.RotateTowards(_appearHackingZoneTrigger.transform.position);
                }
                _hackingArea.SetActive(!_isHacking);
                return;
            }
            SetState(true);
        }
    }

    public void Close()
    {
        if (!_triggered)
            return;

        SetState(false);
    }

    public void Open()
    {
        if (_triggered)
            return;

        SetState(true);
    }

    public void Lock(bool value)
    {
        _hacked = !value;
        _appearHackingZoneTrigger.SetActive(value);
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (!_triggered && other.GetComponent<EnemyAI>() != null)
    //        Open();
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (_triggered && other.GetComponent<EnemyAI>() != null)
    //    {
    //        SetState(false);
    //    }
    //}

    private void Hack()
    {
        _isHacking = true;
        SoundManager.Instanse.PlayLoop(Sound.DoorMasterKey);
        void ActionDone()
        {
            Lock(false);
            _isHacking = false;
            var xp = Randomizator.GetRandomValue(_minMaxXP);
            Stats.Instanse.AddXP(xp);
            PlayResourceAnimation?.Invoke(ResourceType.Money, 0, xp);
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
