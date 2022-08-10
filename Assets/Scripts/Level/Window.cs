using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField] private Transform bottomPoint;

    private MovementController movementController;
    private Lockable lockable;

    private void Awake()
    {
        lockable = GetComponent<Lockable>();
    }

    private void Start()
    {
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();

        void OpenEvent()
        {
            GetComponent<MovingFurnitureElements>().Move();
        }

        void AfterOpeningEvent()
        {
            movementController.JumpThroughWindow(bottomPoint.position);
        }

        lockable.SetEvents(OpenEvent, AfterOpeningEvent);
    }
}
