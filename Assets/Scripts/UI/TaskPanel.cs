using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanel : MonoBehaviour
{
    [SerializeField] private float _animationDuration;
    [SerializeField] private Color[] _cellColors;
    [SerializeField] private Transform _panel;
    [SerializeField] private Image _itemCell;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TMP_Text _taskText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private UIBar _progressBar;

    public void SetBarValue(int value, int maxValue) => _progressBar.SetValue(value, maxValue);

    public void Show(Sprite itemSprite, string taskText, int rewardMoney)
    {
        void Refresh()
        {
            SetItemSprite(itemSprite);
            SetTaskText(taskText);
            SetReward(rewardMoney);
        }

        if (_panel.localScale == Vector3.one)
        {
            DOTween.Sequence()
            .Append(_panel.DOScale(Vector3.one + new Vector3(0.25f, 0.25f, 0.25f), 0.1f))
            .Append(_panel.DOScale(Vector3.zero, _animationDuration))
            .AppendCallback(Refresh)
            .Append(_panel.DOScale(Vector3.one, _animationDuration))
            .Play();
        }
        else
        {
            Refresh();
            _panel.DOScale(Vector3.one, _animationDuration);
        }
    }

    private void Awake()
    {
        _panel.gameObject.SetActive(true);
        _panel.localScale = Vector3.zero;
    }

    private void SetItemSprite(Sprite value)
    {
        _itemIcon.sprite = value;
        _itemCell.color = _cellColors[Random.Range(0, _cellColors.Length)];
    }

    private void SetTaskText(string value) => _taskText.text = value;

    private void SetReward(int value) => _rewardText.text = $"+{value}";
}
