using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewResourceAnimation : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;

    public void SetIcon(Sprite value) => icon.sprite = value;

    public void SetText(string value) => text.text = value;
}
