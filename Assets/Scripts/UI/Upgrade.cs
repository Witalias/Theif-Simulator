using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    [Serializable]
    private class Effect
    {
        public float Value;
        public int Cost;
    }

    public static event Action Upgraded;

    [SerializeField] private UpgradeType _type;
    [SerializeField] private Effect[] _effects;
    [SerializeField] private int _level = 1;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private TMP_Text _increaseTo;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private UIBar _upgradeBar;
    [SerializeField] private Button _buyButton;

    public bool IsMaxLevel => _level > _effects.Length;
    public int Level => _level;
    public UpgradeType Type => _type;

    public void CheckInteractableBuyButton()
    {
        _buyButton.interactable = !IsMaxLevel && Stats.Instanse.Money >= _effects[_level - 1].Cost;
    }

    private void Awake()
    {
        _buyButton.onClick.AddListener(OnBuyButtonPressed);
    }

    public void Initialize(int level)
    {
        _level = Mathf.Clamp(level, 1, _effects.Length + 1);

        if (_level > 1)
            Stats.Instanse.SetUpgradableValue(_type, _effects[_level - 2].Value);

        Refresh();
        CheckInteractableBuyButton();
    }

    private void OnBuyButtonPressed()
    {
        if (IsMaxLevel || Stats.Instanse.Money < _effects[_level - 1].Cost)
            return;

        SoundManager.Instanse.Play(Sound.Buy);
        Stats.Instanse.AddMoney(-_effects[_level - 1].Cost);
        Stats.Instanse.SetUpgradableValue(_type, _effects[_level - 1].Value);
        _level++;
        Refresh();
        Upgraded?.Invoke();
        MetricaSender.PlayerUpgrade(_type, _level);
        TaskManager.Instance.ProcessTask(TaskType.BuyUpgrade, 1);
        TaskManager.Instance.ProcessTask(TaskType.TutorialBuyUpgrade, 1);
    }

    private void Refresh()
    {
        var upgradeValue =  IsMaxLevel ? _effects[_level - 2].Value : _effects[_level - 1].Value;
        SetIncreaseToText((float)Math.Round(upgradeValue, 2));
        SetCostText(IsMaxLevel ? Translation.GetMaximumName() : _effects[_level - 1].Cost.ToString());
        SetBarValue(_level - 1, _effects.Length);
    }

    private void SetIncreaseToText(float value)
    {
        _increaseTo.text = $"{Translation.GetIncreaseToName()}:";
        if (IsMaxLevel)
            _increaseTo.text = $"{Translation.GetIncreaseToNameInPast()}:";
        _increaseTo.text += $" <color=#{ColorUtility.ToHtmlStringRGB(_highlightColor)}>{value}</color>";
    }

    private void SetCostText(string value) => _costText.text = $"{value}";

    private void SetBarValue(float value, float maxValue) => _upgradeBar.SetValue(value, maxValue);

}
