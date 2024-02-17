using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Values", menuName = "Scriptable Objects/Upgrade Values", order = 1)]
public class UpgradeValues : ScriptableObject
{
    [Header("The first value is the default")]
    public Upgrade[] Data;

    [Serializable]
    public class Upgrade
    {
        public float Value;
        public int Cost;
    }
}
