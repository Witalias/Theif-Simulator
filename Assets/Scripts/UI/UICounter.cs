using UnityEngine;
using TMPro;

public class UICounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;

    private Color _defaultColor;

    public void SetValue(float value, float maxValue = -1.0f) => SetValue(value, maxValue);

    public void SetValue(int value, int maxValue = -1)
    {
        valueText.text = value.ToString();
        if (maxValue >= 0)
            valueText.text += $"/{maxValue}";
    }

    public void SetDefaultColor() => valueText.color = _defaultColor;

    public void SetColor(Color value) => valueText.color = value;

    private void Awake()
    {
        _defaultColor = valueText.color;
    }
}
