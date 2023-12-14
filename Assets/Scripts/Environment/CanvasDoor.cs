using UnityEngine;

public class CanvasDoor : MonoBehaviour
{
    private void Start()
    {
        if (transform.eulerAngles.y != 0.0f)
        {
            var scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
    }
}
