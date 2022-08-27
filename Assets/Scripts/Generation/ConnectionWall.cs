using UnityEngine;

public class ConnectionWall : MonoBehaviour
{
    private const string openAnimatorBool = "Open";

    [SerializeField] private Transform passagePoint;
    [SerializeField] private Transform outsidePoint;
    [SerializeField] private GameObject door;

    [Header("Testing")]
    [SerializeField] private float hearingRadius;

    private Animator doorAnimator;
    private LevelGenerator generator;
    private TriggerZone triggerZone;
    private Lockable lockable;

    private bool triggered = false;
    private bool fixedUpdateDone = false;
    private Transform player;

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
        player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).transform;

        void AfterOpeningEvent()
        {
            CheckPresenceDoorAnimator();
            triggered = true;
            triggerZone.RemoveTrigger();
            doorAnimator.SetBool(openAnimatorBool, true);

            var enemies = Physics.OverlapSphere(player.position, 
                GameSettings.Instanse.HearingRadiusAfterOpeningDoor, 
                GameStorage.Instanse.EnemyMask);

            foreach (var enemy in enemies)
            {
                var enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                    enemyAI.SetTargetPoint();
            }
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
            doorAnimator.SetBool(openAnimatorBool, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggered && (other.GetComponent<MovementController>() != null || other.GetComponent<EnemyAI>() != null))
        {
            triggered = false;
            doorAnimator.SetBool(openAnimatorBool, false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(passagePoint.position, hearingRadius);
    }

    private void CheckPresenceDoorAnimator()
    {
        if (doorAnimator == null)
            doorAnimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }
}
