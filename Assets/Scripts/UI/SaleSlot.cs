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

    private int Count => GameData.Instanse.Backpack.GetResourceCount(_type);

    public void Initialize(ResourceType type)
    {
        _type = type;
        _titleText.text = Translation.GetResourceName(type);
        _icon.sprite = GameData.Instanse.GetResourceSprite(type);
        _sellButton.onClick.AddListener(Sell);
        _initialized = true;
    }

    public void Refresh()
    {
        if (!_initialized)
            return;

        _countText.text = Count.ToString();

        _reward = Count * GameData.Instanse.GetResourcePrice(_type);
        _rewardText.text = $"{Translation.GetRewardName()}: <color=#{ColorUtility.ToHtmlStringRGB(_rewardColor)}>{_reward}</color> <sprite index=0>";

        _sellButton.interactable = _reward > 0;
    }

    private void Sell()
    {
        if (!_initialized)
            return;

        TaskManager.Instance.ProcessTask(TaskType.SellItems, GameData.Instanse.Backpack.GetResourceCount(_type));
        TaskManager.Instance.ProcessTask(TaskType.TutorialSellItems, 1);
        TaskManager.Instance.ProcessTask(TaskType.SellCertainItems, _type, GameData.Instanse.Backpack.GetResourceCount(_type));
        GameData.Instanse.Backpack.ClearResource(_type);
        GameData.Instanse.AddSoldItemsCount(Count);
        GameData.Instanse.AddMoney(_reward);
        SoundManager.Instanse.Play(Sound.GetMoney);
        Refresh();
    }
}
