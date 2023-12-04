using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICounter : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Image _icon;

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

    public void SetIcon(Sprite value) => _icon.sprite = value;

    private void Awake()
    {
        _defaultColor = valueText.color;
    }
}
