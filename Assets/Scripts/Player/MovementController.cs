using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MovementController : MonoBehaviour
{
    private const string runAnimatorBool = "Run";
    private const string jumpAnimatorTrigger = "Jump";
    private const string stopJumpAnimatorTrigger = "Stop Jump";
    private const float climbDetectionDistance = 0.1f;

    [SerializeField] private float manuallyMovingSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform questionAppearancePoint;

    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PathTrajectory pathTrajectory;
    private Lootable targetObject;
    private WaitingAndAction waitingAndAction;

    private bool isMoving = false;

    public bool Busy { get; set; } = false;

    public Vector3 QuestionAppearancePoint { get => questionAppearancePoint.position; }

    public void GoToObject(Vector3 point, Lootable obj)
    {
        if (obj == targetObject)
            return;

        AbortSearching();
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
        capsuleCollider.enabled = false;

        void actionAfter()
        {
            Busy = false;
            animator.SetTrigger(stopJumpAnimatorTrigger);
            capsuleCollider.enabled = true;
        }

        var direction = new Vector3(targetPoint.x, transform.position.y, targetPoint.z) - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        var path = new Queue<Vector3>(new List<Vector3>
        {
            transform.position,
            targetPoint,
            transform.position + direction * 2.5f
        });
        pathTrajectory.Go(transform, path, climbSpeed, false, actionAfter, climbDetectionDistance);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        pathTrajectory = GetComponent<PathTrajectory>();
    }

    private void Start()
    {
        waitingAndAction = GameObject.FindGameObjectWithTag(Tags.TimeCircle.ToString()).GetComponent<WaitingAndAction>();
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
            AbortSearching();

            if (agent.enabled) agent.ResetPath();
            agent.enabled = false;
            targetObject = null;

            if (!pathTrajectory.Finished) return;

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
        targetObject.TakeLoot(() => targetObject = null);
    }

    private void AbortSearching()
    {
        waitingAndAction.Abort();
        if (targetObject != null)
        {
            var targetIllumination = targetObject.GetComponent<Illumination>();
            if (targetIllumination != null)
                targetIllumination.Enabled = true;
        }
    }
}