using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class UIHackingAction : MonoBehaviour
{
    private const string ANIMATION_TAP = "Pulsate";
    private const string ANIMATION_HOLD = "Slow Pulsate";

    [SerializeField] private GameObject _content;
    [SerializeField] private TMP_Text _actionText;
    [SerializeField] private TMP_Text _titleText;

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
        _titleText.text = Translation.GetHackingName();
        _actionText.text = Translation.GetTapTapName();
        _animation.Play(ANIMATION_TAP);
        SetActive(active);
    }

    private void Hold(bool active)
    {
        _titleText.text = Translation.GetTheftName();
        _actionText.text = $"{Translation.GetHoldName()}!";
        _animation.Play(ANIMATION_HOLD);
        SetActive(active);
    }

    private void SetActive(bool value)
    {
        _content.SetActive(value);

        if (value == true)
        {
            _titleText.transform.localScale = Vector3.zero;
            _titleText.transform.DOScale(Vector3.one, 0.2f);
            StartCoroutine(PrintPoints());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator PrintPoints()
    {
        var wait = new WaitForSeconds(0.7f);
        var pointsCount = 3;

        while (true)
        {
            for (var i = 0; i < pointsCount; i++)
            {
                yield return wait;
                _titleText.text += ".";
            }
            yield return wait;
            _titleText.text = _titleText.text.Remove(_titleText.text.Length - pointsCount);
        }
    }
}
