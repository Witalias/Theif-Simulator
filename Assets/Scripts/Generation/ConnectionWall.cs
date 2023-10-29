using UnityEngine;

[RequireComponent(typeof(TriggerZone))]
[RequireComponent(typeof(Lockable))]
[RequireComponent(typeof(Noisy))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class ConnectionWall : MonoBehaviour
{
    private const string openAnimatorBool = "Open";

    [SerializeField] private Transform passagePoint;
    [SerializeField] private Transform outsidePoint;
    [SerializeField] private GameObject door;

    private Animator doorAnimator;
    private TriggerZone triggerZone;
    private Lockable lockable;
    private Noisy noisy;
    private AudioSource audioSource;

    private bool triggered = false;

    public void RotateDoor(float value)
    {
        doorAnimator.transform.localRotation = Quaternion.Euler(0, value, 0);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        triggerZone = GetComponent<TriggerZone>();
        lockable = GetComponent<Lockable>();
        noisy = GetComponent<Noisy>();
        audioSource = GetComponent<AudioSource>();
        doorAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        void AfterOpeningEvent()
        {
            triggered = true;
            triggerZone.RemoveTrigger();
            doorAnimator.SetBool(openAnimatorBool, true);
            var hearingRadius = GameSettings.Instanse.HearingRadiusAfterOpeningDoor;
            noisy.Noise(hearingRadius + hearingRadius * Stats.Instanse.IncreasedDoorNoiseInPercents / 100f);
            SoundManager.Instanse.Play(Sound.DoorOpen, audioSource);
        }

        lockable.SetEvents(null, AfterOpeningEvent);
        lockable.Open();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!triggered && other.GetComponent<EnemyAI>() != null)
        {
            triggered = true;
            if (doorAnimator != null)
            {
                doorAnimator.SetBool(openAnimatorBool, true);
                SoundManager.Instanse.Play(Sound.DoorOpen, audioSource);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggered && (other.GetComponent<MovementController>() != null || other.GetComponent<EnemyAI>() != null))
        {
            triggered = false;
            if (doorAnimator != null)
            {
                doorAnimator.SetBool(openAnimatorBool, false);
                SoundManager.Instanse.Play(Sound.DoorClose, audioSource);
            }
        }
    }
}
