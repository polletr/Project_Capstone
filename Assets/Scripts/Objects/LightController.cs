using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class LightController : MonoBehaviour
{
    private Light lightSource;

    [Header("Flicker Settings")]
    [SerializeField] public float minFlickerDuration = 1f; // Minimum flicker duration
    [SerializeField] public float maxFlickerDuration = 3f; // Maximum flicker duration
    [SerializeField] public float flickerFrequency = 0.1f; // Frequency of flickering
    [SerializeField] bool constantFlickering;

    private bool isFlickering = false; // Track flickering state
    //private float flickerChance = 0.3f; // 30% chance to flicker when turning on

    private float originalIntensity;

    [field: SerializeField] public bool GuidingLight = false;

    private EventInstance constantFlickeringSound;

    private Rigidbody rb;
    private SphereCollider sphereCollider;

    private List<Light> childLights = new();

    private CheckLightMaterial checkLight;

    [SerializeField] private UnityEvent OnTurnOffLight;
    [SerializeField] private float constantFlickerInterval;
    private void Awake()
    {
        lightSource = GetComponent<Light>();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("LightController");
        checkLight = GetComponentInParent<CheckLightMaterial>();

        foreach (Light light in GetComponentsInChildren<Light>())
        {
            childLights.Add(light);
        }

        if (lightSource == null)
        {
            Debug.LogError("Error: No Light component found on " + gameObject.name);
            enabled = false;
            return;
        }

        originalIntensity = lightSource.intensity;

        if (constantFlickering)
        {
            constantFlickeringSound = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.LightConstantFlickering);
            constantFlickeringSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            StartCoroutine(ConstantFlickerCoroutine());
        }
    }

    // Turn light on or off
    public void TurnOnOffLight(bool check)
    {
        if (!check && constantFlickering)
            StopConstantFlickering();
        else
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.LightTurnOn, transform.position);
        // Random chance to flicker when turning on/off
        //if (check && Random.value < flickerChance) // When turning on
        lightSource.enabled = check;
        //FlickerLight();
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.LightFlickerOnce, transform.position);

        if (checkLight != null)
            checkLight.ChangeLight(check);

        foreach (Light light in childLights)
        {
            light.enabled = check;
        }

        if (!lightSource.enabled)
            OnTurnOffLight.Invoke();

    }

    public void WaitAndTurnOnLight(float delay)
    {
        StartCoroutine(WaitAndTurnOnLightCoroutine(delay));
    }

    private IEnumerator WaitAndTurnOnLightCoroutine(float delay)
    {
        // Wait for the specified time.
        yield return new WaitForSeconds(delay);

        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.LightTurnOn, transform.position);

        lightSource.enabled = true;
        FlickerLight();

        foreach (Light light in childLights)
        {
            light.enabled = true;
        }

    }
    public void WaitAndTurnOffLight(float delay)
    {
        StartCoroutine(WaitAndTurnOffLightCoroutine(delay));
    }


    private IEnumerator WaitAndTurnOffLightCoroutine(float delay)
    {
        // Wait for the specified time.
        yield return new WaitForSeconds(delay);

        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.LightTurnOn, transform.position);

        lightSource.enabled = false;

        foreach (Light light in childLights)
        {
            light.enabled = false;
        }
        FlickerLight();

        OnTurnOffLight.Invoke();


    }

    // Flicker the light for a specified duration
    public void FlickerLight()
    {
        if (!isFlickering)
        {
            float randomFlickerDuration = Random.Range(minFlickerDuration, maxFlickerDuration);
            StartCoroutine(FlickerCoroutine(randomFlickerDuration));
        }
    }

    private IEnumerator FlickerCoroutine(float maxTime)
    {
        float timer = 0f;
        bool currentEnabled = lightSource.enabled;

        StopConstantFlickering();
        while (timer < maxTime)
        {
            // Randomize the intensity
            lightSource.intensity = Random.Range(0.2f, originalIntensity); // Adjust the max intensity as needed

            // Randomize the time interval for the next flicker
            float flickerTimer = Random.Range(flickerFrequency, flickerFrequency * 2);

            // Randomly turn the light on or off for a more dramatic effect
            lightSource.enabled = (Random.value > 0.3f); // 70% chance to stay on

            foreach (Light light in childLights)
            {
                light.enabled = (Random.value > 0.3f); // 70% chance to stay on;
            }


            timer += flickerTimer;

            // Wait for the next flicker
            yield return new WaitForSeconds(flickerTimer);
        }

        lightSource.enabled = currentEnabled;
        lightSource.intensity = originalIntensity;
        isFlickering = false; // Reset flickering state
    }

    // Constant flicker coroutine
    private IEnumerator ConstantFlickerCoroutine()
    {

        constantFlickeringSound.start();
        while (constantFlickering)
        {
            // Start the flicker effect for `flickerDuration` seconds
            float flickerEndTime = Time.time + 1f;
            while (Time.time < flickerEndTime)
            {
                // Randomize the intensity
                lightSource.intensity = Random.Range(0.5f, originalIntensity);

                // Randomly turn the light on or off
                lightSource.enabled = (Random.value > 0.1f); // 90% chance to stay on

                // Short delay between each flicker within the flicker effect
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
            }

            // Wait for the specified interval before starting the next flicker session
            yield return new WaitForSeconds(constantFlickerInterval);
        }
        constantFlickeringSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }

    private void StopConstantFlickering()
    {
        PLAYBACK_STATE playbackState;
        constantFlickeringSound.getPlaybackState(out playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
        {
            constantFlickeringSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        constantFlickering = false;


    }

    // Change light color
    public void ChangeLightColor(Color newColor)
    {
        lightSource.color = newColor;
    }
}