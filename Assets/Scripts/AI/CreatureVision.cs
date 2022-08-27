using UnityEngine;
using UnityEngine.AI;

public class CreatureVision : MonoBehaviour
{
    [SerializeField] [Range(0, 360)] private float viewAngle = 90f;
    [SerializeField] private float viewDistance = 15f;
    [SerializeField] private float detectionDistance = 3f;
    [SerializeField] private Transform enemyEye;

    private MovementController target;
    //private NavMeshAgent agent;
    //private float rotationSpeed;
    //private Transform agentTransform;

    public bool SeesTarget { get; private set; }

    private void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        //agent.updateRotation = false;
        //rotationSpeed = agent.angularSpeed;
        //agentTransform = agent.transform;
    }

    private void Update()
    {
        var distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        SeesTarget = distanceToPlayer <= detectionDistance || IsInView();
        DrawViewState();
    }

    private bool IsInView() // true если цель видна
    {
        var realAngle = Vector3.Angle(enemyEye.forward, target.QuestionAppearancePoint.position - enemyEye.position);
        if (Physics.Raycast(enemyEye.transform.position, target.QuestionAppearancePoint.position - enemyEye.position, out RaycastHit hit, viewDistance))
        {
            if (realAngle < viewAngle / 2f && Vector3.Distance(enemyEye.position, target.QuestionAppearancePoint.position) <= viewDistance && hit.transform == target.transform)
                return true;
        }
        return false;
    }
    //private void RotateToTarget() // поворачивает в стороно цели со скоростью rotationSpeed
    //{
    //    Vector3 lookVector = target.position - agentTransform.position;
    //    lookVector.y = 0;
    //    if (lookVector == Vector3.zero) return;
    //    agentTransform.rotation = Quaternion.RotateTowards
    //        (
    //            agentTransform.rotation,
    //            Quaternion.LookRotation(lookVector, Vector3.up),
    //            rotationSpeed * Time.deltaTime
    //        );

    //}

    //private void MoveToTarget()
    //{
    //    agent.SetDestination(target.position);
    //}

    private void DrawViewState()
    {
        var left = enemyEye.position + Quaternion.Euler(new Vector3(0, viewAngle / 2f, 0)) * (enemyEye.forward * viewDistance);
        var right = enemyEye.position + Quaternion.Euler(-new Vector3(0, viewAngle / 2f, 0)) * (enemyEye.forward * viewDistance);
        Debug.DrawLine(enemyEye.position, left, Color.yellow);
        Debug.DrawLine(enemyEye.position, right, Color.yellow);
    }


}
