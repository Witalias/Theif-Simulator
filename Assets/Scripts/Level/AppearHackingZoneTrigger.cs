using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AppearHackingZoneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _hackingZone;

    private void Awake()
    {
        _hackingZone.SetActive(false);
    }

    private void Start()
    {
        GetComponent<SphereCollider>().radius = GameSettings.Instanse.AppearHackingZonesDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovementController>() == null)
            return;

        _hackingZone.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovementController>() == null)
            return;

        _hackingZone.SetActive(false);
    }
}
