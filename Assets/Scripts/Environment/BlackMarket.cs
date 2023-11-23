using System;
using UnityEngine;

public class BlackMarket : MonoBehaviour
{
    public static event Action PlayerStayedOnBlackMarket;

    public void OnPlayerEnter()
    {
        PlayerStayedOnBlackMarket?.Invoke();
    }
}
