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
    }

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>();
    }

    private void Update()
    {
        if (triggerZone.Triggered && Input.GetKeyDown(Controls.Instanse.GetKey(ActionControls.OpenClose)))
        {
            CheckPresenceDoorAnimator();
            triggered = true;
            triggerZone.RemoveTrigger();
            doorAnimator.SetBool(openAnimatorBool, true);
        }
    }

    private void FixedUpdate()
    {
        if (generator.Generated && !fixedUpdateDone)
        {
            CheckPresenceDoorAnimator();
            doorAnimator.enabled = true;
            fixedUpdateDone = true;
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
