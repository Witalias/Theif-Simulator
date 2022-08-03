using UnityEngine;
using TMPro;

public class UIHotkey : MonoBehaviour
{
    private const string hideAnimationName = "Hide";
    private const string showAnimationName = "Show";

    [SerializeField] private TextMeshProUGUI text;

    private Animation anim;

    public void SetKey(KeyCode value) => text.text = value.ToString();

    public void Show() => anim.Play(showAnimationName);

    public void Hide() => anim.Play(hideAnimationName);

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }
}
