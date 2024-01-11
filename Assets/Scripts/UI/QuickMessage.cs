using DG.Tweening;
using TMPro;
using UnityEngine;

public class QuickMessage : MonoBehaviour
{
    [SerializeField] private float _animationDuration;
    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        _text.transform.localScale = Vector3.zero;
        _text.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        EnemyAI.ShowQuickMessage += Process;
        Lootable.ShowQuickMessage += Process;
        Stats.ShowQuickMessage += Process;
    }

    private void OnDisable()
    {
        EnemyAI.ShowQuickMessage -= Process;
        Lootable.ShowQuickMessage -= Process;
        Stats.ShowQuickMessage -= Process;
    }

    private void Process(string message, float delay)
    {
        _text.text = message;
        _text.transform.DOScale(Vector3.one, _animationDuration);
        DOVirtual.DelayedCall(delay, () => _text.transform.DOScale(Vector3.zero, _animationDuration));
    }
}
