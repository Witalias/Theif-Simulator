using TMPro;
using UnityEngine;

public class XPBar : MonoBehaviour
{
    [SerializeField] private UIBar _bar;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private ParticleSystem _newLevelParticles;

    public void SetProgress(float value, float maxValue) => _bar.SetValue(value, maxValue);

    public void SetLevel(int value) => _levelText.text = value.ToString();

    public void ActiveConfetti() => _newLevelParticles.Play(true);

    public void SetMaxLevelState()
    {
        _progressText.text = Translation.GetMaximumName();
        SetProgress(1.0f, 1.0f);
    }
}
