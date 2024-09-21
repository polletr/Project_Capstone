using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlashLight : MonoBehaviour
{

    [SerializeField] private float range;
    [SerializeField] private Color lightColor;
    [SerializeField] private float intensity;

    [SerializeField] private float cost;
    [SerializeField] private float batteryLife;


    [SerializeField] private FlashlightAbility[] flashlightAbilities;

    [SerializeField] private FlashlightAbility currentAbility;

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
        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;

        currentAbility = flashlightAbilities[0];

        foreach (FlashlightAbility ability in flashlightAbilities)
        {
            ability.Initialize(this);
        }
    }

    private void Update()
    {

        // Decrease BatteryLife continuously over time based on Cost per second
        batteryLife -= cost * Time.deltaTime;

        if (batteryLife < 0)
        {
            batteryLife = 0;
            // Add any logic for what happens when battery is depleted
            // Turn off the flashlight
        }
    }

    public void HandleSphereCast()
    {
        Ray ray = new Ray(transform.position, transform.forward * range);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 2f, range);
        foreach (RaycastHit hit in hits)
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out IEffectable thing))
                ApplyCurrentAbilityEffect(obj);
        }
    }

    public void ResetLight(float cost)
    {
        StartCoroutine(Flicker(1f));

        batteryLife -= cost;
    }

    public void HandleFlashAblility()
    {

        if (currentAbility != null)
            currentAbility.OnUseAbility();
    }

    public void StopUsingFlashlight()
    {
        if (currentAbility != null)
            currentAbility.OnStopAbility();
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
        Debug.Log($"Current Ability: {currentAbility.GetType().Name}");

    }

    private void ApplyCurrentAbilityEffect(GameObject obj)
    {
        switch (currentAbility)
        {
            case MoveAbility moveAbility:
                if (obj.TryGetComponent(out IMovable moveObj))
                    moveObj.ApplyEffect();
                break;
            case RevealAbility revealAbility:
                if (obj.TryGetComponent(out IRevealable revealObj))
                    revealObj.ApplyEffect();
                break;
                default:
                Debug.Log("This ability is not on");
                    break;
        }
    }

    IEnumerator Flicker(float maxTime)
    {
        float timer = 0f;
        while (timer < maxTime)
        {
            // Randomize the intensity
            Light.intensity = Random.Range(0.2f, intensity);

            // Randomize the time interval for the next flicker
            flickerTimer = Random.Range(minFlickerTime, maxFlickerTime);

            // Randomly turn the light on or off for a more dramatic effect
            Light.enabled = (Random.value > 0.3f); // 70% chance to stay on

            timer += flickerTimer;
            // Wait for the next flicker
            yield return new WaitForSeconds(flickerTimer);
        }

        Light.enabled = true;
        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;
        GetComponentInParent<PlayerController>().ChangeState(new PlayerMoveState());

    }

}


public enum FlashlightAbilityType
{
    None,
    Range,
    Reveal,
    Move,

}


