using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] EventReference audioClip;

    EventInstance audioInstance;

    [SerializeField] UnityEvent PlayFromStart;

    public GameEvent gameEvent;

    private void OnEnable()
    {
        gameEvent.OnPlayerDeath += StopAudioInstance;
    }

    private void OnDisable()
    {
        gameEvent.OnPlayerDeath -= StopAudioInstance;
    }
    private void Start()
    {
        PlayFromStart.Invoke();
    }
    public void PlayOneShotAudio(Transform soundPoint)
    {
        AudioManagerFMOD.Instance.PlayOneShot(audioClip, soundPoint.position);
    }

    public void PlayAudioInstance()
    {
        audioInstance = AudioManagerFMOD.Instance.CreateEventInstance(audioClip);
        audioInstance.start();
    }

    public void Play3DAudio()
    {
        audioInstance = AudioManagerFMOD.Instance.CreateEventInstance(audioClip);
        audioInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        audioInstance.start();
    }


    public void StopAudioInstance()
    {
        PLAYBACK_STATE playbackState;
        audioInstance.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.PLAYING)
        {
            Debug.Log("Stopping Audio");
            audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            audioInstance.release();
        }
    }

}
