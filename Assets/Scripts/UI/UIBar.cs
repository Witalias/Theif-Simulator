using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI title;

    public bool Filled { get => bar.fillAmount >= 0.99f; }

    public void SetValue(float value, float maxValue)
    {
        value = Mathf.Clamp(value, 0.0f, maxValue);
        bar.fillAmount = value / maxValue;
        if (valueText != null)
            valueText.text = $"{(int)value}";
        if (maxValue > 0.0f)
            valueText.text += $"/{(int)maxValue}";
    }

    public void SetTitle(string value)
    {
        if (title == null)
            return;

        title.text = value;
    }
}
