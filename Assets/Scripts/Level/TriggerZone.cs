using System;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private event Action<MovementController> OnEnter;
    private event Action<MovementController> OnExit;
    private event Action<MovementController> OnStay;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out var player))
            OnEnter?.Invoke(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out var player))
            OnExit?.Invoke(player);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out var player))
            OnStay?.Invoke(player);
    }

    public void SubscribeOnEnter(Action<MovementController> action) => OnEnter += action;

    public void UnsubscribeOnEnter(Action<MovementController> action) => OnEnter -= action;

    public void SubscribeOnExit(Action<MovementController> action) => OnExit += action;

    public void UnsubscribeOnExit(Action<MovementController> action) => OnExit -= action;

    public void SubscribeOnStay(Action<MovementController> action) => OnStay += action;

    public void UnsubscribeOnStay(Action<MovementController> action) => OnStay -= action;
}
