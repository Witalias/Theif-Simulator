using System;
using UnityEngine;

public class PoliceStation : MonoBehaviour
{
    public static event Action PlayerInPolice;

    public void OnPlayerEnter()
    {
        PlayerInPolice?.Invoke();
    }
}
