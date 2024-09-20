using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlashLight : MonoBehaviour
{

    [field: SerializeField] public float Range { get; set; }
    [field: SerializeField] public Color LightColor { get; set; }
    [field: SerializeField] public float Intensity { get; set; }

    [field: SerializeField] public float Cost { get; set; }
    [field: SerializeField] public float BatteryLife { get; set; }


    [SerializeField] private FlashlightAbility[] flashlightAbilities;

    private FlashlightAbility currentAbility;

    public Light Light { get; set; }

    [SerializeField]
    private float minFlickerTime = 0.1f;  // Minimum time between flickers
    [SerializeField]
    public float maxFlickerTime = 0.5f;  // Maximum time between flickers

    private float flickerTimer;

    private void Awake()
    {
        Light = GetComponent<Light>();
        Light.enabled = true;
        Light.range = Range;
        Light.intensity = Intensity;
        Light.color = LightColor;

        currentAbility = flashlightAbilities[0];
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward * Range);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 2f, Range);
        foreach (RaycastHit hit in hits)
        {
            var obj = hit.collider.gameObject;
            //obj.GetComponent<FlashlightStrategy>().Execute();
        }

        // Decrease BatteryLife continuously over time based on Cost per second
        BatteryLife -= Cost * Time.deltaTime;

        if (BatteryLife < 0)
        {
            BatteryLife = 0;
            // Add any logic for what happens when battery is depleted
            // Turn off the flashlight
        }
    }

    public void ResetLight(float cost)
    {
        StartCoroutine(Flicker(1f));

        BatteryLife -= cost;
    }

    public void HandleFlashAblility()
    {
        if (currentAbility != null)
            currentAbility.OnUseAbility();
    }

    public void ChangeSelectedAbility(int direction) // Fixed typo in method name
    {
        int currentIndex = Array.IndexOf(flashlightAbilities, currentAbility);


        // Update index based on direction (circular switching)
        currentIndex += direction;

        // Circular switching
        if (currentIndex >= flashlightAbilities.Length)
        {
            currentIndex = 0;
        }
        else if (currentIndex < 0)
        {
            currentIndex = flashlightAbilities.Length - 1;
        }

        // Update currentAbility to the new selected ability
        currentAbility = flashlightAbilities[currentIndex];
    }

    IEnumerator Flicker(float maxTime)
    {
        float timer = 0f;
        while (timer < maxTime)
        {
            // Randomize the intensity
            Light.intensity = Random.Range(0.2f, Intensity);

            // Randomize the time interval for the next flicker
            flickerTimer = Random.Range(minFlickerTime, maxFlickerTime);

            // Randomly turn the light on or off for a more dramatic effect
            Light.enabled = (Random.value > 0.3f); // 70% chance to stay on

            timer += flickerTimer;
            // Wait for the next flicker
            yield return new WaitForSeconds(flickerTimer);
        }

        Light.enabled = true;
        Light.range = Range;
        Light.intensity = Intensity;
        Light.color = LightColor;

    }

}


public enum FlashlightAbilityType
{
    None,
    Range,
    Reveal,
    Move,

}


