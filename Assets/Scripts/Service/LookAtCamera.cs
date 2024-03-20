using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;

    private Transform _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(_mainCamera.position + _offset - transform.position);
    }
}
