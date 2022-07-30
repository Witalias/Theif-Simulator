using UnityEngine;

public class WithoutParent : MonoBehaviour
{
    private LevelGenerator generator;
    private bool done = false;

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>();
    }

    private void Update()
    {
        if (generator.Generated && !done)
        {
            transform.parent = transform.parent.parent;
            done = true;
        }
    }
}
