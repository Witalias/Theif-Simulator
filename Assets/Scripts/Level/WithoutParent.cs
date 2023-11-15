using UnityEngine;

public class WithoutParent : MonoBehaviour
{
    private void Start()
    {
        if (transform.parent != null)
            transform.parent = transform.parent.parent;
    }
}
