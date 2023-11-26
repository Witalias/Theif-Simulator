using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaleSlot : MonoBehaviour
{
    [SerializeField] private Color _rewardColor;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _sellButton;

    private bool _initialized;
    private ResourceType _type;
    private int _reward;

    public void Initialize(ResourceType type)
    {
        _type = type;
        _titleText.text = Translation.GetResourceName(type);
        _icon.sprite = GameStorage.Instanse.GetResourceSprite(type);
        _sellButton.onClick.AddListener(Sell);
        _initialized = true;
    }

    public void Refresh()
    {
        if (!_initialized)
            return;

        var count = Stats.Instanse.GetResourceCount(_type);
        _countText.text = count.ToString();

        _reward = count * GameStorage.Instanse.GetResourcePrice(_type);
        _rewardText.text = $"REWARD: <color=#{ColorUtility.ToHtmlStringRGB(_rewardColor)}>{_reward}</color> <sprite index=0>";

        _sellButton.interactable = _reward > 0;
    }

    private void Sell()
    {
        if (!_initialized)
            return;

        Stats.Instanse.ClearResource(_type);
        Stats.Instanse.AddMoney(_reward);
        SoundManager.Instanse.Play(Sound.GetMoney);
        Refresh();
    }
}
