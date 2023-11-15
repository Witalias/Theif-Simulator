using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float wheelSharpness = 10f;
    [SerializeField] private float minY = 20f;
    [SerializeField] private float maxY = 60f;
    [SerializeField] private float scrollSpeed = 1000f;
    [SerializeField] private MovementController movementController;

    private float currentY = 50f;
    private float sharpness;

    private void Awake()
    {
        sharpness = wheelSharpness;
    }

    private void Update()
    {
       // var wheelValue = Input.GetAxis("Mouse ScrollWheel");
        var wheelValue = 0f;
        if (wheelValue != 0f)
        {
            sharpness = wheelSharpness;
            currentY -= wheelValue * Time.deltaTime * scrollSpeed;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }
    }

    private void FixedUpdate()
    {
        var toPosition = new Vector3(
            movementController.transform.position.x,
            currentY,
            movementController.transform.position.z - 15f);
        transform.position = Vector3.Lerp(transform.position, toPosition, sharpness * Time.fixedDeltaTime);
    }
}
