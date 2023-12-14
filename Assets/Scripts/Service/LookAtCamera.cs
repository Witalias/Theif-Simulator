using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(_mainCamera.position - transform.position);
    }
}
