using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerFMOD : Singleton<AudioManagerFMOD>
{
    public SFXEvents SFXEvents;
    public GameEvent GameEvent;


    private List<EventInstance> eventInstances = new List<EventInstance>();
    private Dictionary<EventReference, EventInstance> bgMusicInstances = new Dictionary<EventReference, EventInstance>();

    // Play one-shot sound effect at a specific position
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    // Create an EventInstance and store it in the list
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        if (!eventInstances.Contains(eventInstance))
            eventInstances.Add(eventInstance);

        return eventInstance;
    }

    private void CleanUp()
    {
        foreach (var eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        eventInstances.Clear();

    }

    public void PauseAllAudio(bool isPaused)
    {
        foreach (var eventInstance in eventInstances)
        {
            eventInstance.setPaused(isPaused);
        }

    }


    protected override void OnDestroy()
    {
        CleanUp();
        base.OnDestroy();
    }
}