using UnityEngine;

public class Fog : MonoBehaviour
{
    public void Remove()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Animation>().Play();
    }
}
