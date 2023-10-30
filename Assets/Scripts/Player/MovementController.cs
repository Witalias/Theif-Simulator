using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PathTrajectory))]
public class MovementController : MonoBehaviour
{
    private const string runAnimatorBool = "Run";
    private const string jumpAnimatorTrigger = "Jump";
    private const string stopJumpAnimatorTrigger = "Stop Jump";
    private const float climbDetectionDistance = 0.1f;

    [SerializeField] private float manuallyMovingSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private FloatingJoystick _joystick;

    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PathTrajectory pathTrajectory;
    private TargetObject targetObject;
    private WaitingAndAction waitingAndAction;

    private bool isMoving = false;
    private float initialAgentSpeed;
    private float initialRigitbodySpeed;

    public bool Busy { get; set; } = false;

    public Transform CenterPoint { get => centerPoint; }

    public bool IsRunning => rb.velocity.magnitude > 0;

    public void GoToObject(Vector3 point, TargetObject obj, RaycastHit hit)
    {
        if (obj == targetObject)
            return;

        targetObject = obj;
        AbortSearching();
        var distance = Vector3.Distance(transform.position, new Vector3(point.x, transform.position.y, point.z));
        if (distance <= obj.ArriveDistance)
        {
            InteractWithTargetObject();
            return;
        }
        if (hit.collider.GetComponent<MovementController>() == null)
        {
            targetObject = null;
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

    public void AddSpeed(float valueInPercents)
    {
        manuallyMovingSpeed = initialRigitbodySpeed + initialRigitbodySpeed * valueInPercents / 100f;
        agent.speed = initialAgentSpeed + initialAgentSpeed * valueInPercents / 100f;
    }

    public void RotateTowards(Vector3 point)
    {
        var direction = point - transform.position;
        var angle = Quaternion.LookRotation(direction).eulerAngles;
        transform.DORotate(new Vector3(0f, angle.y, 0f), 0.5f);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        pathTrajectory = GetComponent<PathTrajectory>();

        initialAgentSpeed = agent.speed;
        initialRigitbodySpeed = manuallyMovingSpeed;
    }

    private void Start()
    {
        waitingAndAction = GameObject.FindGameObjectWithTag(Tags.TimeCircle.ToString()).GetComponent<WaitingAndAction>();
        AddSpeed(Stats.Instanse.IncreasedPlayerSpeedInPercents);
    }

    private void OnEnable()
    {
        WaitingAndAction.TimerActived += OnTimerActived;
    }

    private void OnDisable()
    {
        WaitingAndAction.TimerActived -= OnTimerActived;
    }

    private void Update()
    {
        if (targetObject != null)
        {
            var currentDistance = Vector3.Distance(transform.position, agent.destination);
            if (currentDistance <= targetObject.ArriveDistance)
            {
                InteractWithTargetObject();
                targetObject = null;
                isMoving = false;
                agent.enabled = false;
                animator.SetBool(runAnimatorBool, false);
            }
        }
    }

    private void FixedUpdate()
    {
        var movementVector = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);
        if (movementVector == Vector3.zero)
            movementVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        var direction = new Vector3(movementVector.x, rb.velocity.y, movementVector.z);
        if (direction.magnitude > 1)
            direction.Normalize();

        rb.velocity = direction * manuallyMovingSpeed;

        if (movementVector != Vector3.zero)
        {
            AbortSearching();

            if (agent.enabled) agent.ResetPath();
            agent.enabled = false;
            targetObject = null;

            if (!pathTrajectory.Finished) return;

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

    private void InteractWithTargetObject()
    {
        TryChooseSecurityCamera();
    }
    
    private void TryChooseSecurityCamera()
    {
        var device = targetObject.GetComponent<Device>();
        if (device != null)
            device.OpenActionMenu();
    }

    private void AbortSearching()
    {
        waitingAndAction.Abort();
    }

    private void OnTimerActived(bool value)
    {
        //_joystick.enabled = !value;
    }
}
