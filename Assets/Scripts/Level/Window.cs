using UnityEngine;

public class Window : MonoBehaviour
{
    private TriggerZone triggerZone;
    private MovementController movementController;
    private CenteredPoint centeredPoint;

    public bool Opened { get; private set; } = false;

    public void Open()
    {
        GetComponent<MovingFurnitureElements>().Move();
        Opened = true;
    }

    private void Awake()
    {
        triggerZone = GetComponent<TriggerZone>();
        centeredPoint = GetComponent<CenteredPoint>();
    }

    private void Start()
    {
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

    private void Update()
    {
        if (triggerZone.Triggered && Input.GetKeyDown(Controls.Instanse.GetKey(ActionControls.OpenClose)))
        {
            if (Opened)
                movementController.JumpThroughWindow(centeredPoint.CenterPoint);
            else
                Open();
        }
    }
}
