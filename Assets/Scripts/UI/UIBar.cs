using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI title;

    /// <param name="value">От 0 до 100</param>
    public void SetValue(float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);
        bar.fillAmount = value / 100f;
        if (valueText != null)
            valueText.text = $"{(int)value} %";
    }

    public void SetTitle(string value) => title.text = value;
}
