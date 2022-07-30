using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sharpness = 3f;
    [SerializeField] private float minY = 20f;
    [SerializeField] private float maxY = 60f;
    [SerializeField] private float scrollSpeed = 1000f;
    [SerializeField] private MovementController movementController;

    private float currentY = 50f;

    private void Update()
    {
        var wheelValue = Input.GetAxis("Mouse ScrollWheel");
        if (wheelValue != 0f)
        {
            currentY -= wheelValue * Time.deltaTime * scrollSpeed;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }

        var toPosition = new Vector3(movementController.transform.position.x, currentY, movementController.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, toPosition, sharpness * Time.deltaTime);
    }

}
