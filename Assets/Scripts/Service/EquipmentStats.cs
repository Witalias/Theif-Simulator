using UnityEngine;

public class EquipmentStats : MonoBehaviour
{
    [SerializeField] private EquipmentType type;
    [SerializeField] private Sprite icon;
    [SerializeField] private float hackingTimeDoor = 10f;
    [SerializeField] private float hackingTimeWindow = 10f;
    [SerializeField] private float hackingTimeDevice = 10f;
    [SerializeField] private LoudnessType loudnessType = LoudnessType.Quietly;

    public EquipmentType Type { get => type; }

    public Sprite Icon { get => icon; }

    public float HackingTimeDoor { get => hackingTimeDoor; set => hackingTimeDoor = Mathf.Clamp(value, 1f, 99f); }

    public float HackingTimeWindow { get => hackingTimeWindow; set => hackingTimeWindow = Mathf.Clamp(value, 1f, 99f); }

    public float HackingTimeDevice { get => hackingTimeDevice; set => hackingTimeDevice = Mathf.Clamp(value, 1f, 99f); }

    public LoudnessType LoudnessType { get => loudnessType; set => loudnessType = value; }
}
