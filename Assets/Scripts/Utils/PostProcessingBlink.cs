using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class PostProcessingBlink : MonoBehaviour
{
    [SerializeField] private VolumeProfile blinkVolumeProfile; // List of volume profiles to switch between
    [SerializeField] private float blinkDuration = 0.5f; // Duration for the blink

    private Volume volume; // Reference to the Volume component
    private VolumeProfile originalProfile; // Stores the original profile to switch back to

    void Start()
    {
        volume = GetComponent<Volume>();

        originalProfile = volume.profile; // Store the current profile as the default
    }

    public void BlinkPostProcessingOnce()
    {
        if (blinkVolumeProfile != null)
        {
            StartCoroutine(BlinkOnce(blinkVolumeProfile));
        }
    }

    private IEnumerator BlinkOnce(VolumeProfile blinkProfile)
    {
        // Switch to the selected blinking profile
        volume.profile = blinkProfile;

        // Wait for the blink duration
        yield return new WaitForSeconds(blinkDuration);

        // Switch back to the original profile
        volume.profile = originalProfile;
    }
}
