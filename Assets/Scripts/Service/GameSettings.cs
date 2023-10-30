using UnityEngine;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instanse { get; private set; } = null;

    [SerializeField] private Language language = Language.Russian;
    [SerializeField] private float _tapBonusTimePercents = 5f;

    public Language Language { get => language; set => language = value; }

    public float TapBonusTimePercents => _tapBonusTimePercents;

    public ResourceType GetResourceTypeByEquipmentType(EquipmentType type)
    {
        return type switch
        {
            EquipmentType.MasterKey => ResourceType.MasterKeys,
            EquipmentType.TierIron => ResourceType.TierIrons,
            EquipmentType.Gadget => ResourceType.Gadgets,
            _ => throw new System.Exception($"The resource {type} does not exist"),
        };
    }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);
    }
}
