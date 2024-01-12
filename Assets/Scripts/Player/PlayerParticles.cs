using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private ParticleSystem _smokeParticle;
    [SerializeField] private Transform _flashParticlesParent;
    [SerializeField] private ParticleSystem[] _flashParticles;

    private Vector3 _smokeParticlesLocalPosition;

    public void ActivateSmokeParticles(bool value)
    {
        if (value == true)
        {
            _smokeParticle.transform.parent = _bodyTransform;
            _smokeParticle.transform.localPosition = _smokeParticlesLocalPosition;
            _smokeParticle.Play();
        }
        else
        {
            _smokeParticle.transform.parent = null;
            _smokeParticle.Stop();
        }
    }

    public void ActivateRandomFlashParticle()
    {
        var particle = _flashParticles[Random.Range(0, _flashParticles.Length)];
        particle.transform.position = _flashParticlesParent.position;
        particle.Play();
    }

    private void Start()
    {
        foreach (var particle in _flashParticles)
            particle.transform.parent = null;
        _smokeParticlesLocalPosition = _smokeParticle.transform.localPosition;
    }

    private void OnEnable()
    {
        Lootable.Looted += ActivateRandomFlashParticle;
        Door.Hacked += ActivateRandomFlashParticle;
    }

    private void OnDisable()
    {
        Lootable.Looted -= ActivateRandomFlashParticle;
        Door.Hacked -= ActivateRandomFlashParticle;
    }
}
