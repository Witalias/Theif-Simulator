using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EquipmentStats : MonoBehaviour
{
    [SerializeField] private EquipmentType type;
    [SerializeField] private Sprite icon;
    [SerializeField] private float hackingTimeDoor = 10f;
    [SerializeField] private float hackingTimeWindow = 10f;
    [SerializeField] private float hackingTimeDevice = 10f;
    [SerializeField] private Sound doorSound;
    [SerializeField] private Sound windowSound;
    [SerializeField] private Sound deviceSound;
    [SerializeField] private LoudnessType loudnessType = LoudnessType.Quietly;

    private float initialHackingTimeDoor;
    private float initialHackingTimeWindow;
    private float initialHackingTimeDevice;

    public EquipmentType Type { get => type; }

    public Sprite Icon { get => icon; }

    public float HackingTimeDoor { get => hackingTimeDoor; private set => hackingTimeDoor = Mathf.Clamp(value, 1f, 99f); }

    public float HackingTimeWindow { get => hackingTimeWindow; private set => hackingTimeWindow = Mathf.Clamp(value, 1f, 99f); }

    public float HackingTimeDevice { get => hackingTimeDevice; private set => hackingTimeDevice = Mathf.Clamp(value, 1f, 99f); }

    public Sound DoorSound { get => doorSound; }

    public Sound WindowSound { get => windowSound; }

    public Sound DeviceSound { get => deviceSound; }

    public LoudnessType LoudnessType { get => loudnessType; set => loudnessType = value; }

    public AudioSource AudioSource { get; private set; }

    public float IncreasedHackingTimeInPercents { get; set; } = 0;

    public float IncreasedNoiseInPercents { get; set; } = 0;

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();

        initialHackingTimeDoor = HackingTimeDoor;
        initialHackingTimeWindow = hackingTimeWindow;
        initialHackingTimeDevice = hackingTimeDevice;
    }
}
