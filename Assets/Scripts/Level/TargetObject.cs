using UnityEngine;

[RequireComponent(typeof(CenteredPoint))]
public class TargetObject : MonoBehaviour
{
    [SerializeField] private float arriveDistance = 3f;

    private CenteredPoint centeredPoint;

    public float ArriveDistance { get => arriveDistance; }

    private void Awake()
    {
        centeredPoint = GetComponent<CenteredPoint>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(centeredPoint.CenterPoint, arriveDistance);
    }
}
