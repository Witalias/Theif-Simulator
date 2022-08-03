using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField] private Transform bottomPoint;
    
    private TriggerZone triggerZone;
    private MovementController movementController;

    public bool Opened { get; private set; } = false;

    public void Open()
    {
        GetComponent<MovingFurnitureElements>().Move();
        Opened = true;
    }

    private void Awake()
    {
        triggerZone = GetComponent<TriggerZone>();
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
            {
                movementController.JumpThroughWindow(bottomPoint.position);
                triggerZone.RemoveTrigger();
            }
            else
                Open();
        }
    }
}
