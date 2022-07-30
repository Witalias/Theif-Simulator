using UnityEngine;

public class ResourcesPanel : MonoBehaviour
{
    [SerializeField] private UIBar foodBar;
    [SerializeField] private UIBar waterBar;
    [SerializeField] private UICounter moneyCounter;
    [SerializeField] private UICounter fuelCounter;

    public void SetResourceValue(ResourceType type, float value)
    {
        switch (type)
        {
            case ResourceType.Food: foodBar.SetValue(value); break;
            case ResourceType.Water: waterBar.SetValue(value); break;
            case ResourceType.Money: moneyCounter.SetValue(value); break;
            case ResourceType.Fuel: fuelCounter.SetValue(value); break;
        }
    }
}
