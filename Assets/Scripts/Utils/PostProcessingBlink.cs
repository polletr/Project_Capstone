using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class PostProcessingBlink : MonoBehaviour
{
    [SerializeField] private VolumeProfile blinkVolumeProfile; // List of volume profiles to switch between

    private Volume volume; // Reference to the Volume component
    private VolumeProfile originalProfile; // Stores the original profile to switch back

    [SerializeField] EventReference blinkTraumas;
    private EventInstance traumasInstance;

    void Start()
    {
        volume = GetComponent<Volume>();
        originalProfile = volume.profile; // Store the current profile as the default

        // Initialize the FMOD event instance for the trauma sound
        traumasInstance = AudioManagerFMOD.Instance.CreateEventInstance(blinkTraumas);
    }

    public void BlinkPostProcessingOnce()
    {
        if (blinkVolumeProfile != null)
        {
            volume.profile = blinkVolumeProfile; // Switch to the blink profile
            StartCoroutine(BlinkOnce()); // Start the coroutine to manage the blink
        }
    }

    private IEnumerator BlinkOnce()
    {
        // Start playing the trauma sound
        traumasInstance.start();

        // Check the playback state
        PLAYBACK_STATE playbackState;
        do
        {
            traumasInstance.getPlaybackState(out playbackState);
            yield return null; // Wait for the next frame
        }
        while (playbackState != PLAYBACK_STATE.STOPPED);

        // Once the sound has stopped, switch back to the original profile
        volume.profile = originalProfile;
    }

    private void OnDestroy()
    {
        // Release the FMOD event instance when the object is destroyed
        traumasInstance.release();
    }
}
