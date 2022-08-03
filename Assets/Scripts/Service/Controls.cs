using UnityEngine;
using System.Collections.Generic;

public class Controls : MonoBehaviour
{
    public static Controls Instanse { get; private set; } = null;

    private Dictionary<ActionControls, KeyCode> controlKeys;

    public KeyCode GetKey(ActionControls action) => controlKeys[action];

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        controlKeys = new Dictionary<ActionControls, KeyCode>
        {
            [ActionControls.OpenClose] = KeyCode.E
        };
    }
}
