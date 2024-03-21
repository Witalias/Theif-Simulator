using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    public static event Action Upgraded;

    [SerializeField] private UpgradeType _type;
    [SerializeField] private UpgradeValues _values;
    [SerializeField] private Effect[] _effects;
    [SerializeField] private int _level = 1;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private TMP_Text _increaseTo;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private UIBar _upgradeBar;
    [SerializeField] private Button _buyButton;

    public bool IsMaxLevel => _level > _values.Data.Length;
    public int Level => _level;
    public UpgradeType Type => _type;

    public void CheckInteractableBuyButton()
    {
        _buyButton.interactable = !IsMaxLevel && GameData.Instanse.Money >= _values.Data[_level].Cost;
    }

    private void Awake()
    {
        _buyButton.onClick.AddListener(OnBuyButtonPressed);
    }

    public void Initialize(int level)
    {
        _level = Mathf.Clamp(level, 1, _values.Data.Length);

        GameData.Instanse.SetUpgradableValue(_type, _values.Data[_level - 1].Value);

        Refresh();
        CheckInteractableBuyButton();
    }

    private void OnBuyButtonPressed()
    {
        if (IsMaxLevel || GameData.Instanse.Money < _values.Data[_level].Cost)
            return;

        AudioManager.Instanse.Play(AudioType.Buy);
        GameData.Instanse.AddMoney(-_values.Data[_level].Cost);
        GameData.Instanse.SetUpgradableValue(_type, _values.Data[_level].Value);
        _level++;
        Refresh();
        Upgraded?.Invoke();
        MetricaSender.PlayerUpgrade(_type, _level);
        TaskManager.Instance.ProcessTask(TaskType.BuyUpgrade, 1);
        TaskManager.Instance.ProcessTask(TaskType.TutorialBuyUpgrade, 1);
    }

    private void Refresh()
    {
        var upgradeValue =  IsMaxLevel ? _values.Data[_level - 1].Value : _values.Data[_level].Value;
        SetIncreaseToText((float)Math.Round(upgradeValue, 2));
        SetCostText(IsMaxLevel ? Translation.GetMaximumName() : _values.Data[_level].Cost.ToString());
        SetBarValue(_level - 1, _values.Data.Length);
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

    [Serializable]
    public class Effect
    {
        public float Value;
        public int Cost;
    }
}
