using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instanse { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource loopAudioSource;

    [Header("Sounds")]
    [SerializeField] private AudioClip openMenu;
    [SerializeField] private AudioClip mouseEnterButton;
    [SerializeField] private AudioClip[] selectButton;
    [SerializeField] private AudioClip[] doorClose;
    [SerializeField] private AudioClip[] doorOpen;
    [SerializeField] private AudioClip[] food;
    [SerializeField] private AudioClip[] water;
    [SerializeField] private AudioClip[] getMoney;
    [SerializeField] private AudioClip[] fuel;
    [SerializeField] private AudioClip[] masterKey;
    [SerializeField] private AudioClip[] tierIron;
    [SerializeField] private AudioClip[] gadget;
    [SerializeField] private AudioClip[] scroll;
    [SerializeField] private AudioClip[] suspectMan;
    [SerializeField] private AudioClip[] suspectWoman;
    [SerializeField] private AudioClip[] notFindMan;
    [SerializeField] private AudioClip[] notFindWoman;
    [SerializeField] private AudioClip[] screamMan;
    [SerializeField] private AudioClip[] screamWoman;
    [SerializeField] private AudioClip[] newVisibilityLevel;
    [SerializeField] private AudioClip[] police;
    [SerializeField] private AudioClip[] toilet;
    [SerializeField] private AudioClip[] doorArms;
    [SerializeField] private AudioClip[] doorMasterKey;
    [SerializeField] private AudioClip[] doorTierIron;
    [SerializeField] private AudioClip[] searchingKitchen;
    [SerializeField] private AudioClip[] searchingWoodFurniture;
    [SerializeField] private AudioClip[] embientForest;
    [SerializeField] private AudioClip[] refrigeratorDoor;
    [SerializeField] private AudioClip[] box;
    [SerializeField] private AudioClip[] windowOpen;
    [SerializeField] private AudioClip[] windowArms;
    [SerializeField] private AudioClip[] windowTierIron;
    [SerializeField] private AudioClip[] newSkill;
    [SerializeField] private AudioClip[] bookFlip;

    private Dictionary<Sound, AudioClip[]> sounds;
    private readonly Dictionary<Sound, AudioSource> currentLoopSounds = new Dictionary<Sound, AudioSource>();

    public void Play(Sound sound, AudioSource source = null)
    {
        if (source == null)
            source = audioSource;

        if (sounds[sound].Length == 0)
        {
            Debug.LogWarning("There is no audio clip " + sound.ToString());
            return;
        }
        source.PlayOneShot(GetRandomClip(sound));
    }

    public void PlayOneStream(Sound sound, AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.PlayOneShot(GetRandomClip(sound));
    }

    public void PlayLoop(Sound sound, AudioSource source = null)
    {
        if (source == null)
            source = loopAudioSource;

        currentLoopSounds.Add(sound, source);
        source.PlayOneShot(GetRandomClip(sound));
    }

    public void Stop(Sound sound)
    {
        if (!currentLoopSounds.ContainsKey(sound))
            return;

        currentLoopSounds[sound].Stop();
        currentLoopSounds.Remove(sound);
    }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        sounds = new Dictionary<Sound, AudioClip[]>
        {
            [Sound.MouseEnterButton] = new[] { mouseEnterButton },
            [Sound.OpenMenu] = new[] { openMenu },
            [Sound.Select] = selectButton,
            [Sound.DoorClose] = doorClose,
            [Sound.DoorOpen] = doorOpen,
            [Sound.Food] = food,
            [Sound.Fuel] = fuel,
            [Sound.Gadget] = gadget,
            [Sound.GetMoney] = getMoney,
            [Sound.MasterKey] = masterKey,
            [Sound.TierIron] = tierIron,
            [Sound.Water] = water,
            [Sound.Scroll] = scroll,
            [Sound.NotFindMan] = notFindMan,
            [Sound.NotFindWoman] = notFindWoman,
            [Sound.ScreamMan] = screamMan,
            [Sound.ScreamWoman] = screamWoman,
            [Sound.SuspectMan] = suspectMan,
            [Sound.SuspectWoman] = suspectWoman,
            [Sound.NewVisibilityLevel] = newVisibilityLevel,
            [Sound.Police] = police,
            [Sound.DoorArms] = doorArms,
            [Sound.DoorMasterKey] = doorMasterKey,
            [Sound.DoorTierIron] = doorTierIron,
            [Sound.Toilet] = toilet,
            [Sound.SearchingKitchen] = searchingKitchen,
            [Sound.SearchingWoodFurniture] = searchingWoodFurniture,
            [Sound.EmbientForest] = embientForest,
            [Sound.RefrigeratorDoor] = refrigeratorDoor,
            [Sound.Box] = box,
            [Sound.WindowOpen] = windowOpen,
            [Sound.WindowArms] = windowArms,
            [Sound.WindowTierIron] = windowTierIron,
            [Sound.NewSkill] = newSkill,
            [Sound.BookFlip] = bookFlip
        };
    }

    private void Update()
    {
        foreach (var element in currentLoopSounds)
        {
            if (element.Value.isPlaying)
                continue;

            element.Value.PlayOneShot(GetRandomClip(element.Key));
        }
    }

    private AudioClip GetRandomClip(Sound sound) => sounds[sound][Random.Range(0, sounds[sound].Length)];
}
