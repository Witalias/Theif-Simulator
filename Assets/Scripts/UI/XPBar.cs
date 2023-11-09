using TMPro;
using UnityEngine;

public class XPBar : MonoBehaviour
{
    [SerializeField] private UIBar _bar;
    [SerializeField] private TMP_Text _levelText;

    public void SetProgress(float value, float maxValue) => _bar.SetValue(value, maxValue);

    public void SetLevel(int value) => _levelText.text = value.ToString();
}
