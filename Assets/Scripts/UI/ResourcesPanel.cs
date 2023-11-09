using UnityEngine;
using TMPro;
using DG.Tweening;

public class ResourcesPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private UICounter _money;
    [SerializeField] private UICounter _bottles;
    [SerializeField] private UICounter _sneakers;
    [SerializeField] private Transform _resourceAnimationPoint;

    private Camera _mainCamera;

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

    private void Awake()
    {
        _mainCamera = Camera.main;
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

    private void UpdatePanels()
    {
        foreach (var panel in _panels)
            panel.SetActive(false);
        DOVirtual.DelayedCall(Time.deltaTime, () =>
        {
            foreach (var panel in _panels)
                panel.SetActive(true);
        });
    }

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
