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

    public void CheckInteractableBuyButton()
    {
        _buyButton.interactable = !IsMaxLevel && Stats.Instanse.Money >= _effects[_level - 1].Cost;
    }

    private void Awake()
    {
        _buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    private void Start()
    {
        _level = Mathf.Clamp(_level, 1, _effects.Length + 1);
        for (var i = 0; i < _level - 1; i++)
            Stats.Instanse.SetUpgradableValue(_type, _effects[i].Value);
        Refresh();
        CheckInteractableBuyButton();
    }

    private void OnBuyButtonClick()
    {
        if (IsMaxLevel || Stats.Instanse.Money < _effects[_level - 1].Cost)
            return;

        SoundManager.Instanse.Play(Sound.GetMoney);
        Stats.Instanse.AddMoney(-_effects[_level - 1].Cost);
        Stats.Instanse.SetUpgradableValue(_type, _effects[_level - 1].Value);
        _level++;
        Refresh();
        Upgraded?.Invoke();
        TaskManager.Instance.ProcessTask(TaskType.BuyUpgrade, 1);
    }

    private void Refresh()
    {
        var upgradeValue =  IsMaxLevel ? 0.0f : _effects[_level - 1].Value;
        SetIncreaseToText((float)Math.Round(upgradeValue, 2));
        SetCostText(IsMaxLevel ? Translation.GetMaximumName() : _effects[_level - 1].Cost.ToString());
        SetBarValue(_level - 1, _effects.Length);
    }

    private void SetIncreaseToText(float value)
    {
        _increaseTo.text = "INCREASE TO:";
        if (IsMaxLevel)
            _increaseTo.text = "INCREASED TO:";
        _increaseTo.text += $" <color=#{ColorUtility.ToHtmlStringRGB(_highlightColor)}>{value}</color>";
    }

    private void SetCostText(string value) => _costText.text = $"{value}";

    private void SetBarValue(float value, float maxValue) => _upgradeBar.SetValue(value, maxValue);

}
