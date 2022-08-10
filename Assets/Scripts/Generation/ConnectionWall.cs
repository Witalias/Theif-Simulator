using UnityEngine;

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

    private void OnTriggerExit(Collider other)
    {
        if (triggered && other.CompareTag(Tags.Player.ToString()))
        {
            triggered = false;
            doorAnimator.SetBool(openAnimatorBool, false);
        }
    }

    private void CheckPresenceDoorAnimator()
    {
        if (doorAnimator == null)
            doorAnimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }
}
