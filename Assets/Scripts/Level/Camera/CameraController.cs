using UnityEngine;

[RequireComponent(typeof(CameraBounds))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float wheelSharpness = 10f;
    [SerializeField] private float boundsSharpness = 3f;
    [SerializeField] private float minY = 20f;
    [SerializeField] private float maxY = 60f;
    [SerializeField] private float scrollSpeed = 1000f;
    [SerializeField] private MovementController movementController;

    private CameraBounds bounds;

    private float currentY = 50f;
    private float sharpness;

    private void Awake()
    {
        bounds = GetComponent<CameraBounds>();
        sharpness = wheelSharpness;
    }

    private void Update()
    {
        var wheelValue = Input.GetAxis("Mouse ScrollWheel");
        if (bounds.InBounds)
        {
            sharpness = boundsSharpness;
            currentY = maxY;
        }
        else if (wheelValue != 0f)
        {
            sharpness = wheelSharpness;
            currentY -= wheelValue * Time.deltaTime * scrollSpeed;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }
    }

    private void FixedUpdate()
    {
        var toPosition = new Vector3(
            bounds.InBoundsX ? transform.position.x : movementController.transform.position.x,
            currentY,
            bounds.InBoundsZ ? transform.position.z : movementController.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, toPosition, sharpness * Time.fixedDeltaTime);
    }
}
