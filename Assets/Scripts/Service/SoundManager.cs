using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instanse { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource loopAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] doorClose;
    [SerializeField] private AudioClip[] doorOpen;
    [SerializeField] private AudioClip[] getResource;
    [SerializeField] private AudioClip[] getSmallResource;
    [SerializeField] private AudioClip[] getLoudResource;
    [SerializeField] private AudioClip[] getMoney;
    [SerializeField] private AudioClip[] suspectMan;
    [SerializeField] private AudioClip[] suspectWoman;
    [SerializeField] private AudioClip[] notFindMan;
    [SerializeField] private AudioClip[] notFindWoman;
    [SerializeField] private AudioClip[] screamMan;
    [SerializeField] private AudioClip[] screamWoman;
    [SerializeField] private AudioClip[] doorMasterKey;
    [SerializeField] private AudioClip[] searchingKitchen;
    [SerializeField] private AudioClip[] searchingWoodFurniture;
    [SerializeField] private AudioClip[] refrigeratorDoor;
    [SerializeField] private AudioClip[] musicTheme;
    [SerializeField] private AudioClip[] call;

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

    public void PlayMusic(Sound sound)
    {
        musicAudioSource.clip = GetRandomClip(sound);
        musicAudioSource.Play();
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
            [Sound.DoorClose] = doorClose,
            [Sound.DoorOpen] = doorOpen,
            [Sound.GetMoney] = getMoney,
            [Sound.GetSmallResource] = getSmallResource,
            [Sound.GetLoudResource] = getLoudResource,
            [Sound.GetResource] = getResource,
            [Sound.NotFindMan] = notFindMan,
            [Sound.NotFindWoman] = notFindWoman,
            [Sound.ScreamMan] = screamMan,
            [Sound.ScreamWoman] = screamWoman,
            [Sound.SuspectMan] = suspectMan,
            [Sound.SuspectWoman] = suspectWoman,
            [Sound.HackDoor] = doorMasterKey,
            [Sound.SearchingKitchen] = searchingKitchen,
            [Sound.SearchingWoodFurniture] = searchingWoodFurniture,
            [Sound.RefrigeratorDoor] = refrigeratorDoor,
            [Sound.MusicTheme] = musicTheme,
            [Sound.Call] = call,
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
