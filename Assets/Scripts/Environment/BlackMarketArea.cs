using System;
using UnityEngine;

public class BlackMarketArea : MonoBehaviour
{
    public static event Action PlayerStayed;

    private bool _triggered;

    public void OnPlayerStay(MovementController player)
    {
        if (_triggered || player.Busy || player.IsRunning)
            return;

        _triggered = true;
        PlayerStayed?.Invoke();
    }

    public void OnPlayExit()
    {
        _triggered = false;
    }
}
