using UnityEngine;
using UnityEngine.AI;

public class CreatureVision : MonoBehaviour
{
    [SerializeField] [Range(0, 360)] private float viewAngle = 90f;
    [SerializeField] private float viewDistance = 15f;
    [SerializeField] private Transform enemyEye;

    private MovementController _target;

    public bool SeesTarget { get; private set; }

    public float ViewAngle { get => viewAngle; set => viewAngle = value; }

    public float ViewDistance { get => viewDistance; set => viewDistance = value; }

    public void Detect()
    {
        SeesTarget = true;
    }

    public void NotDetect()
    {
        SeesTarget = false;
    }

    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

    private void Update()
    {
        SeesTarget = IsInView();
        DrawViewState();
    }

    private bool IsInView()
    {
        var realAngle = Vector3.Angle(enemyEye.forward, _target.CenterPoint.position - enemyEye.position);
        if (Physics.Raycast(enemyEye.transform.position, _target.CenterPoint.position - enemyEye.position, out RaycastHit hit, viewDistance))
        {
            if (realAngle < viewAngle / 2f && Vector3.Distance(enemyEye.position, _target.CenterPoint.position) <= viewDistance && hit.transform == _target.transform)
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
