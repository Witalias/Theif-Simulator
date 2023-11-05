using UnityEngine;
using TMPro;
using DG.Tweening;

public class ResourcesPanel : MonoBehaviour
{
    [SerializeField] private GameObject _moneyPanel;
    [SerializeField] private GameObject _itemsPanel;
    [SerializeField] private UICounter _money;
    [SerializeField] private UICounter _bottles;
    [SerializeField] private UICounter _sneakers;

    public void SetResourceValue(ResourceType type, int value)
    {
        switch (type)
        {
            case ResourceType.Money: _money.SetValue(value); break;
            case ResourceType.Bootle: _bottles.SetValue(value); break;
            case ResourceType.Sneakers: _sneakers.SetValue(value); break;
        }
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        _moneyPanel.SetActive(false);
        _itemsPanel.SetActive(false);
        DOVirtual.DelayedCall(Time.deltaTime, () =>
        {
            _moneyPanel.SetActive(true);
            _itemsPanel.SetActive(true);
        });
    }
}
