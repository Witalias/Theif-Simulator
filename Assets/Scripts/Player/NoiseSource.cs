using UnityEngine;

[RequireComponent(typeof(Noisy))]
public class NoiseSource : MonoBehaviour
{
    private Noisy noisy;

    private void Awake()
    {
        noisy = GetComponent<Noisy>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(Controls.Instanse.GetKey(ActionControls.Noise)))
            noisy.Noise(GameSettings.Instanse.HearingRadiusAfterPlayerNoise, true);
    }
}
