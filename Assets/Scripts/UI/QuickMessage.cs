using DG.Tweening;
using TMPro;
using UnityEngine;

public class QuickMessage : MonoBehaviour
{
    [SerializeField] private float _animationDuration;
    [SerializeField] private Color _positiveColor;
    [SerializeField] private Color _negativeColor;
    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        _text.transform.localScale = Vector3.zero;
        _text.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        HumanAI.ShowQuickMessage += Process;
        Lootable.ShowQuickMessage += Process;
        PlayerLevelController.ShowQuickMessage += Process;
        BackpackController.ShowQuickMessage += Process;
        DogAI.GShowQuickMessage += Process;
        Cheats.GShowQuickMessage += Process;
    }

    private void OnDisable()
    {
        HumanAI.ShowQuickMessage -= Process;
        Lootable.ShowQuickMessage -= Process;
        PlayerLevelController.ShowQuickMessage -= Process;
        BackpackController.ShowQuickMessage -= Process;
        DogAI.GShowQuickMessage -= Process;
        Cheats.GShowQuickMessage -= Process;
    }

    private void Process(string message, float delay, bool isNegative)
    {
        _text.text = message;
        _text.color = isNegative ? _negativeColor : _positiveColor;
        _text.transform.DOScale(Vector3.one, _animationDuration);
        DOVirtual.DelayedCall(delay, () => _text.transform.DOScale(Vector3.zero, _animationDuration));
    }
}
