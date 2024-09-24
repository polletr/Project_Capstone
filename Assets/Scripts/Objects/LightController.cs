using System.Collections;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private Light lightSource;

    [Header("Flicker Settings")]
    [SerializeField] public float minFlickerDuration = 1f; // Minimum flicker duration
    [SerializeField] public float maxFlickerDuration = 3f; // Maximum flicker duration
    [SerializeField] public float flickerFrequency = 0.1f; // Frequency of flickering

    private bool isFlickering = false; // Track flickering state
    private float flickerChance = 0.3f; // 30% chance to flicker when turning on
    private void Awake()
    {
        lightSource = GetComponent<Light>();

        if (lightSource == null)
        {
            Debug.LogError("Error: No Light component found on " + gameObject.name);
            enabled = false;
        }
    }

    // Turn light on or off
    public void TurnOnOffLight(bool check)
    {
        if (lightSource != null)
        {
            lightSource.enabled = check;

            // Random chance to flicker when turning on/off
            if (check && Random.value < flickerChance) // When turning on
            {
                FlickerLight();
            }
        }
    }

    // Flicker the light for a specified duration
    public void FlickerLight()
    {
        if (lightSource != null && !isFlickering)
        {
            float randomFlickerDuration = Random.Range(minFlickerDuration, maxFlickerDuration);
            StartCoroutine(FlickerCoroutine(randomFlickerDuration));
        }
    }

    private IEnumerator FlickerCoroutine(float maxTime)
    {
        float timer = 0f;

        while (timer < maxTime)
        {
            // Randomize the intensity
            lightSource.intensity = Random.Range(0.2f, 1f); // Adjust the max intensity as needed

            // Randomize the time interval for the next flicker
            float flickerTimer = Random.Range(flickerFrequency, flickerFrequency * 2);

            // Randomly turn the light on or off for a more dramatic effect
            lightSource.enabled = (Random.value > 0.3f); // 70% chance to stay on

            timer += flickerTimer;

            // Wait for the next flicker
            yield return new WaitForSeconds(flickerTimer);
        }

        lightSource.enabled = true; // Ensure the light is left on after flickering
        isFlickering = false; // Reset flickering state
    }

    // Change light color
    public void ChangeLightColor(Color newColor)
    {
        if (lightSource != null)
            lightSource.color = newColor;
    }
}