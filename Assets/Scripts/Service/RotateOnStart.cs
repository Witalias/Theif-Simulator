using UnityEngine;

public class RotateOnStart : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;

    private void Start()
    {
        transform.eulerAngles = _rotation;
    }
}
