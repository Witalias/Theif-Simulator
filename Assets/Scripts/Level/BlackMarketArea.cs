using System;
using UnityEngine;

public class BlackMarketArea : MonoBehaviour
{
    public static event Action PlayerStayed;
    public static event Action PlayerExit;

    private bool _triggered;

    public void OnPlayerStay(MovementController player)
    {
        if (player.IsRunning)
        {
            _triggered = false;
            return;
        }
        else if (_triggered || player.Busy)
            return;

        _triggered = true;
        PlayerStayed?.Invoke();
    }

    public void OnPlayExit()
    {
        _triggered = false;
        PlayerExit?.Invoke();
    }
}
