using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private Image differenceBar;
    [SerializeField] private TextMeshProUGUI valueText;

    public bool Filled { get => bar.fillAmount >= 0.99f; }

    public void SetValue(float value, float maxValue)
    {
        value = Mathf.Clamp(value, 0.0f, maxValue);
        if (differenceBar != null)
            differenceBar.fillAmount = value / maxValue;
        DOVirtual.DelayedCall(0.25f, () => bar.DOFillAmount(value / maxValue, 1.0f));
        if (valueText != null)
            valueText.text = $"{(int)value}/{(int)maxValue}";
    }
}
