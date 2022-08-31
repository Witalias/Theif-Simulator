using UnityEngine;
using UnityEngine.AI;

public class CreatureVision : MonoBehaviour
{
    [SerializeField] [Range(0, 360)] private float viewAngle = 90f;
    [SerializeField] private float viewDistance = 15f;
    [SerializeField] private float detectionDistance = 3f;
    [SerializeField] private Transform enemyEye;

    private MovementController target;

    public bool SeesTarget { get; private set; }

    public float ViewAngle { get => viewAngle; set => viewAngle = value; }

    public float ViewDistance { get => viewDistance; set => viewDistance = value; }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

    private void Update()
    {
        var distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        SeesTarget = distanceToPlayer <= detectionDistance || IsInView();
        DrawViewState();
    }

    private bool IsInView() // true если цель видна
    {
        var realAngle = Vector3.Angle(enemyEye.forward, target.CenterPoint.position - enemyEye.position);
        if (Physics.Raycast(enemyEye.transform.position, target.CenterPoint.position - enemyEye.position, out RaycastHit hit, viewDistance))
        {
            if (realAngle < viewAngle / 2f && Vector3.Distance(enemyEye.position, target.CenterPoint.position) <= viewDistance && hit.transform == target.transform)
                return true;
        }
        return false;
    }

    private void DrawViewState()
    {
        var left = enemyEye.position + Quaternion.Euler(new Vector3(0, viewAngle / 2f, 0)) * (enemyEye.forward * viewDistance);
        var right = enemyEye.position + Quaternion.Euler(-new Vector3(0, viewAngle / 2f, 0)) * (enemyEye.forward * viewDistance);
        Debug.DrawLine(enemyEye.position, left, Color.yellow);
        Debug.DrawLine(enemyEye.position, right, Color.yellow);
    }


}
