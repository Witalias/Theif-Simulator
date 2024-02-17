using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AppearHackingZoneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _hackingZone;
    [SerializeField] private ParticleSystem _appearParticle;

    private void OnEnable()
    {
        _hackingZone.SetActive(false);
    }

    private void Start()
    {
        GetComponent<SphereCollider>().radius = GameData.Instanse.AppearHackingZonesDistance;
        _appearParticle.transform.position = _hackingZone.transform.position + new Vector3(0.0f, 2.0f, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovementController>() == null)
            return;

        _hackingZone.SetActive(true);
        _appearParticle.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovementController>() == null)
            return;

        _hackingZone.SetActive(false);
        _appearParticle.Play();
    }
}
