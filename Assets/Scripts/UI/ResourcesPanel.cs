using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class ResourcesPanel : MonoBehaviour
{
    [SerializeField] private Color _fullBackpackTextColor;
    [SerializeField] private GameObject[] _itemCounters;
    [SerializeField] private GameObject _itemPanel;
    [SerializeField] private UICounter _backpack;
    [SerializeField] private UICounter _money;
    [SerializeField] private UICounter _bottles;
    [SerializeField] private UICounter _sneakers;
    [SerializeField] private Transform _resourceAnimationPoint;

    public void SetResourceValue(ResourceType type, int value)
    {
        switch (type)
        {
            case ResourceType.Money: _money.SetValue(value); break;
            case ResourceType.Bootle: _bottles.SetValue(value); break;
            case ResourceType.Sneakers: _sneakers.SetValue(value); break;
        }
        //UpdatePanels();
    }

    public void SetActiveCounter(ResourceType type, bool value)
    {
        switch (type)
        {
            case ResourceType.Money: _money.gameObject.SetActive(value); break;
            case ResourceType.Bootle: _bottles.gameObject.SetActive(value); break;
            case ResourceType.Sneakers: _sneakers.gameObject.SetActive(value); break;
        }
        //UpdatePanels();
    }

    public void SetBackpackCapacity(int fullness, int capacity)
    {
        _backpack.SetValue(fullness, capacity);

        if (fullness >= capacity)
            _backpack.SetColor(_fullBackpackTextColor);
        else
            _backpack.SetDefaultColor();
    }

    public void ClearResources()
    {
        SetActiveCounter(ResourceType.Bootle, false);
        SetActiveCounter(ResourceType.Sneakers, false);
    }

    private void OnEnable()
    {
        Lootable.PlayResourceAnimation += PlayResourceAnimation;
        Door.PlayResourceAnimation += PlayResourceAnimation;
    }

    private void OnDisable()
    {
        Lootable.PlayResourceAnimation -= PlayResourceAnimation;
        Door.PlayResourceAnimation -= PlayResourceAnimation;
    }

    //private void UpdatePanels()
    //{
    //    foreach (var counter in _itemCounters)
    //    {
    //        if (counter.activeSelf)
    //        {
    //            _itemPanel.SetActive(true);
    //            return;
    //        }
    //    }
    //    _itemPanel.SetActive(false);
    //}

    private void PlayResourceAnimation(ResourceType type, int count, int xp)
    {
        if (count > 0)
            CreateResourceAnimation(count.ToString(), GameStorage.Instanse.GetResourceSprite(type));
        DOVirtual.DelayedCall(count > 0 ? 1.0f : 0.0f, () => CreateResourceAnimation(xp.ToString(), GameStorage.Instanse.Star));

        void CreateResourceAnimation(string text, Sprite icon)
        {
            var newResource = Instantiate(GameStorage.Instanse.NewResourceAnimatinPrefab,
                _resourceAnimationPoint.position, Quaternion.identity, transform)
                .GetComponent<NewResourceAnimation>();
            newResource.SetIcon(icon);
            newResource.SetText(text);
        }
    }
}
