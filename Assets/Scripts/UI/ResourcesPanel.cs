using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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

    private readonly Queue<Action> _resourceAnimationQueue = new();
    private Coroutine _playResourceAnimationCoroutine;

    public void SetResourceValue(ResourceType type, int value)
    {
        switch (type)
        {
            case ResourceType.Bottle: _bottles.SetValue(value); break;
            case ResourceType.Sneakers: _sneakers.SetValue(value); break;
        }
        //UpdatePanels();
    }

    public void SetActiveCounter(ResourceType type, bool value)
    {
        switch (type)
        {
            case ResourceType.Bottle: _bottles.gameObject.SetActive(value); break;
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

    public void SetMoney(int value) => _money.SetValue(value);

    public void ClearResources()
    {
        SetActiveCounter(ResourceType.Bottle, false);
        SetActiveCounter(ResourceType.Sneakers, false);
        SetActiveCounter(ResourceType.Diamond, false);
        SetActiveCounter(ResourceType.Watch, false);
        SetActiveCounter(ResourceType.Phone, false);
        SetActiveCounter(ResourceType.Ring, false);
    }

    private void OnEnable()
    {
        Lootable.PlayResourceAnimation += PlayResourceAnimationUniversal;
        Door.PlayResourceAnimationXp += PlayResourceAnimationXp;
        TaskManager.PlayResourceAnimationMoney += PlayResourceAnimationMoney;
    }

    private void OnDisable()
    {
        Lootable.PlayResourceAnimation -= PlayResourceAnimationUniversal;
        Door.PlayResourceAnimationXp -= PlayResourceAnimationXp;
        TaskManager.PlayResourceAnimationMoney -= PlayResourceAnimationMoney;
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

    private void PlayResourceAnimationUniversal(ResourceType resourceType, int count, int xp, int money)
    {
        if (count > 0)
            PlayResourceAnimationItem(resourceType, count);
        if (money > 0)
            PlayResourceAnimationMoney(money);
        if (xp > 0)
            PlayResourceAnimationXp(xp);
    }

    private void PlayResourceAnimationItem(ResourceType type, int count)
    {
        _resourceAnimationQueue.Enqueue(() => CreateResourceAnimation(count.ToString(), GameStorage.Instanse.GetResourceSprite(type)));
        PlayResourceAnimation();
    }

    private void PlayResourceAnimationXp(int count)
    {
        _resourceAnimationQueue.Enqueue(() => CreateResourceAnimation(count.ToString(), GameStorage.Instanse.Star));
        PlayResourceAnimation();
    }

    private void PlayResourceAnimationMoney(int count)
    {
        _resourceAnimationQueue.Enqueue(() => CreateResourceAnimation(count.ToString(), GameStorage.Instanse.Money));
        PlayResourceAnimation();
    }

    private void PlayResourceAnimation()
    {
        if (_playResourceAnimationCoroutine == null)
            _playResourceAnimationCoroutine = StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            var wait = new WaitForSeconds(1.0f);
            while (_resourceAnimationQueue.Count > 0)
            {
                _resourceAnimationQueue.Dequeue()?.Invoke();
                yield return wait;
            }
            _playResourceAnimationCoroutine = null;
        }
    }

    private void CreateResourceAnimation(string text, Sprite icon)
    {
        var newResource = Instantiate(GameStorage.Instanse.NewResourceAnimatinPrefab,
            _resourceAnimationPoint.position, Quaternion.identity, transform)
            .GetComponent<NewResourceAnimation>();
        newResource.SetIcon(icon);
        newResource.SetText(text);
    }
}
