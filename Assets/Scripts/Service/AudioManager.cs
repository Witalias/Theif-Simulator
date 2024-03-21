using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instanse { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource loopAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Sounds")]
    [SerializeField] private AudioData[] _audioData;

    private Dictionary<AudioType, AudioClip[]> _sounds;
    private readonly Dictionary<AudioType, AudioSource> _currentLoopSounds = new Dictionary<AudioType, AudioSource>();

    private void Awake()
    {
        Instanse = this;
        _sounds = _audioData.ToDictionary(audio => audio.Type, audio => audio.Clips);
    }

    private void Update()
    {
        foreach (var element in _currentLoopSounds)
        {
            if (element.Value.isPlaying)
                continue;

            element.Value.PlayOneShot(GetRandomClip(element.Key));
        }
    }

    public void Play(AudioType sound, AudioSource source = null)
    {
        if (source == null)
            source = audioSource;

        if (_sounds[sound].Length == 0)
        {
            Debug.LogWarning("There is no audio clip " + sound.ToString());
            return;
        }
        source.PlayOneShot(GetRandomClip(sound));
    }

    public void PlayOneStream(AudioType sound, AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.PlayOneShot(GetRandomClip(sound));
    }

    public void PlayLoop(AudioType sound, AudioSource source = null)
    {
        if (source == null)
            source = loopAudioSource;

        _currentLoopSounds.Add(sound, source);
        source.PlayOneShot(GetRandomClip(sound));
    }

    public void PlayMusic(AudioType sound)
    {
        musicAudioSource.clip = GetRandomClip(sound);
        musicAudioSource.Play();
    }

    public void Stop(AudioType sound)
    {
        if (!_currentLoopSounds.ContainsKey(sound))
            return;

        _currentLoopSounds[sound].Stop();
        _currentLoopSounds.Remove(sound);
    }

    private AudioClip GetRandomClip(AudioType sound) => _sounds[sound][Random.Range(0, _sounds[sound].Length)];
}
