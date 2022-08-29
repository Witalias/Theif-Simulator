using UnityEngine;
using TMPro;

public class ResourcesPanel : MonoBehaviour
{
    [SerializeField] private UIBar foodBar;
    [SerializeField] private UIBar waterBar;
    [SerializeField] private UICounter moneyCounter;
    [SerializeField] private UICounter fuelCounter;
    [SerializeField] private UICounter masterKeysCounter;
    [SerializeField] private UICounter tierIronsCounter;
    [SerializeField] private UIHotkey noiseHotkey;
    [SerializeField] private TextMeshProUGUI noiseText;

    public void SetResourceValue(ResourceType type, float value)
    {
        switch (type)
        {
            case ResourceType.Food: foodBar.SetValue(value); break;
            case ResourceType.Water: waterBar.SetValue(value); break;
            case ResourceType.Money: moneyCounter.SetValue(value); break;
            case ResourceType.Fuel: fuelCounter.SetValue(value); break;
            case ResourceType.MasterKeys: masterKeysCounter.SetValue(value); break;
            case ResourceType.TierIrons: tierIronsCounter.SetValue(value); break;
        }
    }

    private void Start()
    {
        foodBar.SetTitle(Translation.Get(ResourceType.Food));
        waterBar.SetTitle(Translation.Get(ResourceType.Water));
        moneyCounter.SetTitle(Translation.Get(ResourceType.Money));
        fuelCounter.SetTitle(Translation.Get(ResourceType.Fuel));
        masterKeysCounter.SetTitle(Translation.Get(ResourceType.MasterKeys));
        tierIronsCounter.SetTitle(Translation.Get(ResourceType.TierIrons));

        noiseHotkey.SetKey(Controls.Instanse.GetKey(ActionControls.Noise));
        noiseText.text = Translation.GetNoiseName();
    }
}
