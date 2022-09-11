using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField] private float upLimit;
    [SerializeField] private float bottomLimit;
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;

    private Transform player;

    public bool InBounds { get => InBoundsX || InBoundsZ; }

    public bool InBoundsX { get => player.position.x <= leftLimit || player.position.x >= rightLimit; }

    public bool InBoundsZ { get => player.position.z <= bottomLimit || player.position.z >= upLimit; }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftLimit, 1, upLimit), new Vector3(rightLimit, 1, upLimit));
        Gizmos.DrawLine(new Vector3(leftLimit, 1, bottomLimit), new Vector3(rightLimit, 1, bottomLimit));
        Gizmos.DrawLine(new Vector3(leftLimit, 1, upLimit), new Vector3(leftLimit, 1, bottomLimit));
        Gizmos.DrawLine(new Vector3(rightLimit, 1, upLimit), new Vector3(rightLimit, 1, bottomLimit));
    }
}
