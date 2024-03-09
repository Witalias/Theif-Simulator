using DG.Tweening;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private ParticleSystem _smokeParticle;
    [SerializeField] private ParticleSystem _fastSmokeParticle;
    [SerializeField] private Transform _particlesContainer;
    [SerializeField] private ParticleSystem _confettiParticle;

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

    public void ActivateConfettiParticle()
    {
        _confettiParticle.transform.position = _particlesContainer.position;
        _confettiParticle.Play();
    }

    public void ActivateFastSmokeParticle()
    {
        _fastSmokeParticle.Play();
    }

    private void Start()
    {
        _smokeParticlesLocalPosition = _smokeParticle.transform.localPosition;
        _confettiParticle.transform.parent = null;
    }

    private void OnEnable()
    {
        Lootable.GOnLooted += ActivateConfettiParticle;
        Door.Hacked += ActivateConfettiParticle;
    }

    private void OnDisable()
    {
        Lootable.GOnLooted -= ActivateConfettiParticle;
        Door.Hacked -= ActivateConfettiParticle;
    }
}
