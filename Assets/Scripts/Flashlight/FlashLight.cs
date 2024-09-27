using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class FlashLight : MonoBehaviour
{
    public GameEvent Event;

    [SerializeField] private float range;
    [SerializeField] private Color lightColor;
    [SerializeField] private float intensity;

    [SerializeField] private float cost;
    [field: SerializeField] private Battery battery { get; set; }


    [SerializeField] private FlashlightAbility[] flashlightAbilities;

    [SerializeField] private FlashlightAbility currentAbility;

    public Light Light { get; set; }

    [SerializeField]
    private float minFlickerTime = 0.1f;  // Minimum time between flickers
    [SerializeField]
    public float maxFlickerTime = 0.5f;  // Maximum time between flickers

    private float flickerTimer;

    private PlayerController playerController;
    private PlayerInventory playerInventory;

    bool isFlashlightOn;
    bool isFlickering;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        Light = GetComponent<Light>();
        Light.enabled = true;
        isFlashlightOn = true;
        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;

        currentAbility = flashlightAbilities[0];

/*        foreach (FlashlightAbility ability in flashlightAbilities)
        {
            ability.Initialize(this);
        }
*/    }

    private void OnEnable()
    {
        Event.OnChangeBattery += SetBattery;
    }

    private void OnDisable()
    {
        Event.OnChangeBattery -= SetBattery;
    }

    private void Update()
    {

        if (battery != null)
        {

            // Decrease BatteryLife continuously over time based on Cost per second
            if (isFlashlightOn && !battery.IsBatteryDead())
                battery.BatteryLife -= cost * Time.deltaTime;

            if (battery.IsBatteryDead())
            {
                Debug.Log("Out of Battery");
                // Turn off the flashlight
                if (isFlashlightOn && !isFlickering)
                    StartCoroutine(Flicker(3f, () => TurnOffLight()));
            }
        }
        else
        {
            if (isFlashlightOn && !isFlickering)
                StartCoroutine(Flicker(3f, () => TurnOffLight()));
        }

    }

    public void HandleSphereCast()
    {
        Ray ray = new Ray(transform.position, transform.forward * range);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 2f, range);
        foreach (RaycastHit hit in hits)
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out IEffectable effectable))
                ApplyCurrentAbilityEffect(obj);
        }
    }

    public void ResetLight()
    {
        StartCoroutine(Flicker(1f, () => ResetLightState()));

    }

    public void HandleFlashAblility()
    {
        if (currentAbility != null && isFlashlightOn)
            currentAbility.OnUseAbility();
        else
            playerController.currentState?.HandleMove();
    }

    public void StopUsingFlashlight()
    {
        if (currentAbility != null && isFlashlightOn)
            currentAbility.OnStopAbility();
    }

    private void ResetLightState()
    {
        Light.enabled = true;
        isFlashlightOn = true;
        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;
        playerController.currentState?.HandleMove();
    }

    public void TurnOffLight()
    {
        Light.enabled = false;
        isFlashlightOn = false;
    }

    public void ConsumeBattery(float cost)
    {
        if (!battery.IsBatteryDead())
            battery.Drain(cost);
        else
            Debug.Log("Battery is Dead change it B***H");
    }

    public void TurnOnLight()
    {
        if (battery != null && !battery.IsBatteryDead())
        {
            ResetLightState();
        }
        else
        {
            ResetLightState();
            StartCoroutine(Flicker(1f, () => TurnOffLight()));
        }
    }

    public void HandleFlashlightPower()
    {
        isFlashlightOn = !isFlashlightOn;
        if (isFlashlightOn)
        {
            TurnOnLight();
        }
        else
        {
            TurnOffLight();
        }
    }


    public void ChangeSelectedAbility(int direction) // Fixed typo in method name
    {
        if (flashlightAbilities.Count() > 0)
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

        }
    }

    IEnumerator Flicker(float maxTime, Action onFlickerEnd)
    {
        isFlickering = true;
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

        onFlickerEnd?.Invoke();
        isFlickering = false;
    }

    public void RemoveOldBattery()
    {
        battery = null;
    }

    private void SetBattery(Battery newBattery)
    {
        battery = newBattery;
    }

}

public enum FlashlightAbilityType
{
    None,
    Range,
    Reveal,
    Move,

}


