using UnityEngine;

public class DeleteArrow : MonoBehaviour
{
    [SerializeField] private LayerMask _tutorialMask;

    public void Delete()
    {
        Collider[] colliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, 1.0f, colliders, _tutorialMask);
        foreach (var collider in colliders)
        {
            if (collider != null)
                Destroy(collider.gameObject);
        }
        Destroy(this);
    }
}
