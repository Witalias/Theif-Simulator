using UnityEngine;

[RequireComponent(typeof(TriggerZone))]
[RequireComponent(typeof(Lockable))]
[RequireComponent(typeof(Noisy))]
[RequireComponent(typeof(AudioSource))]
public class ConnectionWall : MonoBehaviour
{
    private const string openAnimatorBool = "Open";

    [SerializeField] private Transform passagePoint;
    [SerializeField] private Transform outsidePoint;
    [SerializeField] private GameObject door;

    private Animator doorAnimator;
    private LevelGenerator generator;
    private TriggerZone triggerZone;
    private Lockable lockable;
    private Noisy noisy;
    private AudioSource audioSource;

    private bool triggered = false;
    private bool fixedUpdateDone = false;

    public Vector3 PassagePointPosition { get => passagePoint.position; }

    public Vector3 OutsidePoint { get => outsidePoint.position; }

    public bool RemoveAfterGeneration { get; set; }

    public void RotateDoor(float value)
    {
        CheckPresenceDoorAnimator();
        doorAnimator.transform.localRotation = Quaternion.Euler(0, value, 0);
    }

    public void Delete()
    {
        if (RemoveAfterGeneration)
            Destroy(gameObject);
    }

    private void Awake()
    {
        triggerZone = GetComponent<TriggerZone>();
        lockable = GetComponent<Lockable>();
        noisy = GetComponent<Noisy>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>();

        void AfterOpeningEvent()
        {
            CheckPresenceDoorAnimator();
            triggered = true;
            triggerZone.RemoveTrigger();
            doorAnimator.SetBool(openAnimatorBool, true);
            var hearingRadius = GameSettings.Instanse.HearingRadiusAfterOpeningDoor;
            noisy.Noise(hearingRadius + hearingRadius * Stats.Instanse.IncreasedDoorNoiseInPercents / 100f);
            SoundManager.Instanse.Play(Sound.DoorOpen, audioSource);
        }

        lockable.SetEvents(null, AfterOpeningEvent);
    }

    private void FixedUpdate()
    {
        if (generator.Generated && !fixedUpdateDone)
        {
            CheckPresenceDoorAnimator();
            doorAnimator.enabled = true;
            fixedUpdateDone = true;
            lockable.Open();
        }
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

    private void CheckPresenceDoorAnimator()
    {
        if (doorAnimator == null)
            doorAnimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }
}
