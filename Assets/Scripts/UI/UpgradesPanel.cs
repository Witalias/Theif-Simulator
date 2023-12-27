using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OpenClosePopup))]
public class UpgradesPanel : MonoBehaviour
{
    [SerializeField] private Upgrade[] _upgradeSlots;

    private readonly Dictionary<UpgradeType, Upgrade> _upgradesDict = new();
    private OpenClosePopup _popup;
    private bool _maxUpgrades;

    private void Awake()
    {
        _popup = GetComponent<OpenClosePopup>();

        foreach (var upgrade in _upgradeSlots)
            _upgradesDict.Add(upgrade.Type, upgrade);
    }

    private void Start()
    {
        LoadLevels();
    }

    private void OnEnable()
    {
        UpgradesPopupButton.Clicked += _popup.Open;
        Upgrade.Upgraded += OnUpgrade;
    }

    private void OnDisable()
    {
        UpgradesPopupButton.Clicked -= _popup.Open;
        Upgrade.Upgraded -= OnUpgrade;
    }

    private void OnUpgrade()
    {
        foreach (var upgrade in _upgradeSlots)
            upgrade.CheckInteractableBuyButton();

        SaveLoad.SaveUpgradeLevels(_upgradeSlots);

        if (_maxUpgrades)
            return;

        foreach (var upgrade in _upgradeSlots)
        {
            if (!upgrade.IsMaxLevel)
                return;
        }
        TaskManager.Instance.RemoveAvailableTask(TaskType.BuyUpgrade);
        _maxUpgrades = true;
    }

    private void LoadLevels()
    {
        var upgradeLevels = SaveLoad.LoadUpgradeLevels();
        if (upgradeLevels == null)
        {
            foreach (var upgrade in _upgradeSlots)
                upgrade.Initialize(1);
        }
        else
        {
            foreach (var upgrade in upgradeLevels)
                _upgradesDict[upgrade.Key].Initialize(upgrade.Value);
        }
    }
}
