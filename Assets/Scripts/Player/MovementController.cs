using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour
{
    private const string runAnimatorBool = "Run";
    private const string jumpAnimatorTrigger = "Jump";
    private const string stopJumpAnimatorTrigger = "Stop Jump";

    [SerializeField] private float manuallyMovingSpeed = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;
    private Lootable targetObject;

    private bool isMoving = false;

    public void GoToObject(Vector3 point, Lootable obj)
    {
        targetObject = obj;
        isMoving = true;
        animator.SetBool(runAnimatorBool, true);
        agent.enabled = true;
        agent.SetDestination(point);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (targetObject != null && transform.position == agent.destination)
        {
            transform.rotation = Quaternion.FromToRotation(transform.position, targetObject.transform.position);
            targetObject.TakeLoot();
            targetObject = null;
            isMoving = false;
            animator.SetBool(runAnimatorBool, false);
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
}
