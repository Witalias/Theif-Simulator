using UnityEngine;
using System;

public class Lockable : MonoBehaviour
{
    [Tooltip("Работает только с заранее установленными объектами")]
    [SerializeField] private bool locked = false;

    private TriggerZone triggerZone;
    private ActionMenu actionMenu;
    private WaitingAndAction waitingAndAction;

    private Action openEvent;
    private Action afterOpeningEvent;

    public bool Opened { get; private set; } = false;

    public bool Locked { get => locked; set => locked = value; }

    public void SetEvents(Action open, Action afterOpen)
    {
        openEvent = open;
        afterOpeningEvent = afterOpen;
    }

    public void Open()
    {
        if (Locked)
            return;

        Opened = true;
        openEvent?.Invoke();
    }

    private void Awake()
    {
        triggerZone = GetComponent<TriggerZone>();
        actionMenu = GetComponent<ActionMenu>();
    }

    private void Start()
    {
        waitingAndAction = GameObject.FindGameObjectWithTag(Tags.TimeCircle.ToString()).GetComponent<WaitingAndAction>();
    }

    private void Update()
    {
        if (triggerZone.Triggered && Input.GetKeyDown(Controls.Instanse.GetKey(ActionControls.OpenClose)) && !waitingAndAction.InProgress)
        {
            if (Opened)
            {
                afterOpeningEvent?.Invoke();
                triggerZone.RemoveTrigger();
            }
            else if (Locked)
            {
                if (actionMenu.Opened)
                {
                    StartCoroutine(actionMenu.Close());
                    triggerZone.ShowHotkey();
                }
                else
                {
                    StartCoroutine(actionMenu.Open());
                    triggerZone.HideHotkey();
                }
            }
            else
                Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovementController>() != null)
        {
            if (actionMenu.Opened)
                StartCoroutine(actionMenu.Close());
            waitingAndAction.Abort();
        }
    }
}
