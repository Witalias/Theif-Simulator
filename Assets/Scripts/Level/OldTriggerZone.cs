using UnityEngine;
using UnityEngine.Events;

public class OldTriggerZone : MonoBehaviour
{
    [SerializeField] private UnityEvent<MovementController> _onEnter;
    [SerializeField] private UnityEvent<MovementController> _onStay;
    [SerializeField] private UnityEvent<MovementController> _onExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out MovementController player))
            _onEnter?.Invoke(player);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out MovementController player))
            _onStay?.Invoke(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out MovementController player))
            _onExit?.Invoke(player);
    }
}
