using UnityEngine;
using System.Collections;

public class SearchingArea : MonoBehaviour
{
    [SerializeField] private Transform searchRayStart;
    [SerializeField] private float searchingInterval = 0.5f;
    [SerializeField] private float searchingDistance = 1f;

    private LayerMask fogMask;
    private LayerMask windowMask;

    private void Awake()
    {
        fogMask = 1 << 11;
        windowMask = 1 << 9;
    }

    private void Start()
    {
        StartCoroutine(Search());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(searchRayStart.position, transform.forward * searchingDistance);
    }

    private IEnumerator Search()
    {
        yield return new WaitForSeconds(searchingInterval);
        //var layer = fogMask | windowMask;
        if (Physics.Raycast(searchRayStart.position, transform.forward, out RaycastHit hit, searchingDistance))
            CheckRaycastHit(hit);
        StartCoroutine(Search());
    }

    private void CheckRaycastHit(RaycastHit hit)
    {
        switch (hit.collider.gameObject.layer)
        {
            case 9:
                if (Physics.Raycast(hit.point + transform.forward, transform.forward, out RaycastHit nextHit, searchingDistance))
                    CheckRaycastHit(nextHit);
                break;
            case 11:
                hit.collider.GetComponent<Fog>().Remove();
                break;
        }
    }
}
