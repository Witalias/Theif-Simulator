using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class UIHackingAction : MonoBehaviour
{
    private const string ANIMATION_TAP = "Pulsate";
    private const string ANIMATION_HOLD = "Slow Pulsate";

    [SerializeField] private string _tapText = "TAP TAP";
    [SerializeField] private string _holdText = "HOLD";
    [SerializeField] private GameObject _content;
    [SerializeField] private TMP_Text _text;

    private Animation _animation;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    private void OnEnable()
    {
        WaitingAndAction.TimerActived += Tap;
        UIHoldButton.HoldButtonActived += Hold;
    }

    private void OnDisable()
    {
        WaitingAndAction.TimerActived -= Tap;
        UIHoldButton.HoldButtonActived -= Hold;
    }

    private void Tap(bool active)
    {
        _text.text = _tapText;
        _animation.Play(ANIMATION_TAP);
        SetActive(active);
    }

    private void Hold(bool active)
    {
        _text.text = _holdText;
        _animation.Play(ANIMATION_HOLD);
        SetActive(active);
    }

    private void SetActive(bool value)
    {
        _content.SetActive(value);
    }
}
