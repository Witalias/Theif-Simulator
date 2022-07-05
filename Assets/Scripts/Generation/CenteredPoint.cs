using UnityEngine;

public class CenteredPoint : MonoBehaviour
{
    [SerializeField] private Transform centerPoint;

    public Vector3 CenterPoint { get => centerPoint.position; }
}
