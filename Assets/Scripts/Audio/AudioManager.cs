using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private AudioMixerGroup bgMusicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource speaker;

    [Header("Main BG Audio Sources")]
    [SerializeField] private List<AudioSource> _mainBGSources = new();

    [Header("Audio Clips")]
    [SerializeField] private AudioClip timeoutClip;

    private List<AudioSource> _audioSources = new();

    private void Awake()
    {
        foreach (AudioSource source in GetComponentsInChildren<AudioSource>())
        {
            _audioSources.Add(source);
        }
    }

    public void PlayAudio(AudioClip clip, bool isBgMusic = false, bool canPlayDuplicates = true)
    {
        if (!canPlayDuplicates)
        {
            foreach (AudioSource source in _audioSources)
            {
                if (source.clip == clip && source.isPlaying)
                    return;
            }
        }
        AudioSource audioSource = GetAvailableAudioSource();

        if (audioSource)
        {
            audioSource.outputAudioMixerGroup = isBgMusic ? bgMusicGroup : sfxGroup;
            audioSource.loop = isBgMusic;
            audioSource.pitch = 1;
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No available audio sources");
        }
    }

 
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (!audioSource.isPlaying && audioSource.outputAudioMixerGroup != bgMusicGroup)
            {
                return audioSource;
            }
        }

        AudioSource newAudioSource = Instantiate(speaker, transform);
        _audioSources.Add(newAudioSource);
        return newAudioSource;
    }


    public void PauseBGAudio()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource.outputAudioMixerGroup == bgMusicGroup)
            {
                audioSource.Pause();
            }
        }
    }
    public void ResumeBGAudio()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource.outputAudioMixerGroup == bgMusicGroup)
            {
                audioSource.UnPause();
                audioSource.pitch = 1;
            }
        }
    }

    public void StopAllBGAudio()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource.outputAudioMixerGroup == bgMusicGroup)
            {
                audioSource.Stop();
            }
        }
    }

    public void StopAllAudio()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.Stop();
        }
    }

    public void PlayTimeoutSound()
    {
        StopAllBGAudio();
        PlayAudio(timeoutClip);
    }

    public void MuteAudio(bool isMuted, float volume)
    {
        mixer.SetFloat("Master", isMuted ? Mathf.Log10(0.001f) * 20 : Mathf.Log10(volume) * 20);
    }

    public void SlowDownBGMusic(float pitchSpeed)
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource.outputAudioMixerGroup == bgMusicGroup)
            {
                audioSource.pitch = pitchSpeed;
            }
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }
}
