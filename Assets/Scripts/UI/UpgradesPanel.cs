using UnityEngine;

[RequireComponent(typeof(OpenClosePopup))]
public class UpgradesPanel : MonoBehaviour
{
    [SerializeField] private Upgrade[] _upgradeSlots;

    private OpenClosePopup _popup;
    private bool _maxUpgrades;

    private void Awake()
    {
        _popup = GetComponent<OpenClosePopup>();
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
}
