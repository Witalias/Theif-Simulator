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
    [SerializeField] private GameObject _itemCounterPrefab;
    [SerializeField] private Transform _itemPanel;
    [SerializeField] private UICounter _money;
    [SerializeField] private UICounter _backpack;
    [SerializeField] private Transform _resourceAnimationPoint;

    private readonly Dictionary<ResourceType, UICounter> _itemCounters = new();
    private readonly Queue<Action> _resourceAnimationQueue = new();
    private Coroutine _playResourceAnimationCoroutine;

    public void SetResourceValue(ResourceType type, int value)
    {
        _itemCounters[type].SetValue(value);
    }

    public void SetActiveCounter(ResourceType type, bool value)
    {
        _itemCounters[type].gameObject.SetActive(value);
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
        foreach (var counter in _itemCounters.Values)
            counter.gameObject.SetActive(false);
    }

    private void Awake()
    {
        CreateItemCounters();
        ClearResources();
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

    private void CreateItemCounters()
    {
        foreach (var resource in Enum.GetValues(typeof(ResourceType)))
        {
            var counter = Instantiate(_itemCounterPrefab, _itemPanel).GetComponent<UICounter>();
            counter.SetIcon(GameStorage.Instanse.GetResourceSprite((ResourceType)resource));
            _itemCounters.Add((ResourceType)resource, counter);
        }
    }

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
        _playResourceAnimationCoroutine ??= StartCoroutine(Coroutine());

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
