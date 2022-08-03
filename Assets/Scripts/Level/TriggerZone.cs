using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public bool Triggered { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovementController>() != null)
            Triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovementController>() != null)
            Triggered = false;
    }
}
