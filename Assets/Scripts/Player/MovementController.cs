using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour
{
    private const string runAnimatorBool = "Run";
    private const string jumpAnimatorTrigger = "Jump";
    private const string stopJumpAnimatorTrigger = "Stop Jump";

    [SerializeField] private float manuallyMovingSpeed = 5f;
    [SerializeField] private LayerMask playerMask;

    private NavMeshAgent agent;
    private Animator animator;
    private Animation anim;
    private Rigidbody rb;
    private Lootable targetObject;

    private bool isMoving = false;

    public bool Busy { get; set; } = false;

    public void GoToObject(Vector3 point, Lootable obj)
    {
        targetObject = obj;
        var distance = Vector3.Distance(transform.position, new Vector3(point.x, transform.position.y, point.z));
        if (distance <= obj.ArriveDistance)
        {
            LootTargetObject();
            return;
        }

        isMoving = true;
        animator.SetBool(runAnimatorBool, true);
        agent.enabled = true;
        agent.SetDestination(point);
    }

    public void StopJumpAnimation()
    {
        animator.SetTrigger(stopJumpAnimatorTrigger);
        Busy = false;
    }

    public void JumpThroughWindow(Vector3 targetPoint)
    {
        if (Busy)
            return;

        Busy = true;
        animator.SetTrigger(jumpAnimatorTrigger);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        anim = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (targetObject != null)
        {
            var currentDistance = Vector3.Distance(transform.position, agent.destination);
            if (currentDistance <= targetObject.ArriveDistance)
            {
                LootTargetObject();
                isMoving = false;
                agent.enabled = false;
                animator.SetBool(runAnimatorBool, false);
            }
        }
    }

    private void FixedUpdate()
    {
        var movementVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (movementVector != Vector3.zero)
        {
            if (agent.enabled) agent.ResetPath();
            agent.enabled = false;
            targetObject = null;
            rb.AddForce(manuallyMovingSpeed * Time.deltaTime * movementVector, ForceMode.Impulse);
            rb.MoveRotation(Quaternion.LookRotation(movementVector));

            if (!isMoving)
            {
                isMoving = true;
                animator.SetBool(runAnimatorBool, true);
            }
        }
        else if (!agent.enabled)
        {
            isMoving = false;
            animator.SetBool(runAnimatorBool, false);
        }
    }

    private void LootTargetObject()
    {
        targetObject.TakeLoot();
        targetObject = null;
    }
}
