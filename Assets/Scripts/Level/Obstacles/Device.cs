using UnityEngine;
using System;

[RequireComponent(typeof(ActionMenu))]
[RequireComponent(typeof(TargetObject))]
public class Device : MonoBehaviour
{
    private ActionMenu actionMenu;
    private MovementController movementController;
    private TargetObject targetObject;
    private CenteredPoint centeredPoint;

    private Action actionAfterTurnedOff;
    
    public bool TurnedOff { get; private set; }

    public void SetEvent(Action actionAfterTurnedOff) => this.actionAfterTurnedOff = actionAfterTurnedOff;

    public void OpenActionMenu()
    {
        if (!actionMenu.Opened)
            StartCoroutine(actionMenu.Open());
    }

    public void TurnOff()
    {
        TurnedOff = true;
        actionAfterTurnedOff?.Invoke();
    }

    private void Awake()
    {
        actionMenu = GetComponent<ActionMenu>();
        targetObject = GetComponent<TargetObject>();
        centeredPoint = GetComponent<CenteredPoint>();
    }

    private void Start()
    {
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

    private void OnMouseDown()
    {
        //if (!TurnedOff && Physics.Raycast(centeredPoint.CenterPoint, movementController.CenterPoint.position - centeredPoint.CenterPoint, out RaycastHit hit))
        //    movementController.GoToObject(centeredPoint.CenterPoint, targetObject, hit);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<MovementController>() != null)
        {
            if (actionMenu.Opened)
                StartCoroutine(actionMenu.Close());
        }
    }
}
