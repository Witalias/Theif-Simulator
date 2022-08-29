using UnityEngine;
using TMPro;

public class UICounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI title;

    public void SetValue(float value) => valueText.text = $"{(int)value}";

    public void SetTitle(string value) => title.text = value;
}
