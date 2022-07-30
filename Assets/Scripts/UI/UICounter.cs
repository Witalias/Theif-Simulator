using UnityEngine;
using TMPro;

public class UICounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;

    public void SetValue(float value) => valueText.text = $"{(int)value}";
}
