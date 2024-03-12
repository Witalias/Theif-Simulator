using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AppearHackingZoneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _hackingZone;
    [SerializeField] private TriggerZone _triggerZone;
    [SerializeField] private ParticleSystem _appearParticle;

    public bool Enabled { get; set; } = true;

    private void Start()
    {
        GetComponent<SphereCollider>().radius = GameData.Instanse.AppearHackingZonesDistance;
        _appearParticle.transform.position = _hackingZone.transform.position + new Vector3(0.0f, 2.0f, 0.0f);
    }

    private void OnEnable()
    {
        _hackingZone.SetActive(false);
        _triggerZone.SubscribeOnEnter(OnEnter);
        _triggerZone.SubscribeOnExit(OnExit);
    }

    private void OnDisable()
    {
        _triggerZone.UnsubscribeOnEnter(OnEnter);
        _triggerZone.UnsubscribeOnExit(OnExit);
    }

    private void OnEnter(MovementController player)
    {
        if (Enabled)
        {
            _hackingZone.SetActive(true);
            _appearParticle.Play();
        }
    }

    private void OnExit(MovementController player)
    {
        if (Enabled)
        {
            _hackingZone.SetActive(false);
            _appearParticle.Play();
        }
    }
}
