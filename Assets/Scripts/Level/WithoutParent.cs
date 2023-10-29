using UnityEngine;

public class WithoutParent : MonoBehaviour
{
    private void Start()
    {
        transform.parent = transform.parent.parent;
    }
}
