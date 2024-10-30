using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

public class FlashLight : MonoBehaviour
{
    public GameEvent Event;

    [Header("Base Flashlight Settings")] [SerializeField]
    private Color lightColor;

    [SerializeField] private float intensity;
    [SerializeField] private float range;
    [SerializeField] private float innerSpotAngle = 15f;
    [SerializeField] private float outerSpotAngle = 30f;

    [Header("Battery Settings")] [SerializeField]
    private float baseCost;

    [SerializeField] private float minFlickerTime = 0.1f; // Minimum time between flickers
    [SerializeField] private float maxFlickerTime = 0.5f; // Maximum time between flickers

    [field: SerializeField] public float BatteryLife { get; private set; }


    [field: SerializeField] public float MaxBatteryLife { get; private set; } = 100;

    public float TotalBatteryLife => MaxBatteryLife + extraCharge;

    [SerializeField] private List<FlashlightAbility> flashlightAbilities;

    public Transform RayCastOrigin => player.PlayerCam.transform;
    public FlashlightAbility CurrentAbility { get; private set; }

    public Light Light { get; private set; }

    [field: SerializeField] public LayerMask IgrnoreMask { get; private set; }

    public bool IsFlashlightOn { get; private set; }

    private float flickerTimer;
    private float extraCharge;
    private bool isFlickering;

    private PlayerController player;

    private List<IEffectable> effectedObjs = new();
    private HashSet<IEffectable> effectedObjsThisFrame = new();

    public bool CanUseAbility { get; private set; } = true;

    private void Awake()
    {
        Light = GetComponent<Light>();

        IsFlashlightOn = Light.enabled;
        Light.intensity = intensity;
        Light.color = lightColor;
        Light.spotAngle = outerSpotAngle;
        Light.innerSpotAngle = innerSpotAngle;
        Light.range = range;

        player = GetComponentInParent<PlayerController>();

        for (var i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out FlashlightAbility ability))
            {
                flashlightAbilities.Add(ability);
            }
        }


        if (flashlightAbilities.Count <= 0) return;

        foreach (var ability in flashlightAbilities.Where(ability => ability != null))
        {
            ability.Initialize(this);
        }

        CurrentAbility = flashlightAbilities[0];
    }

    private void OnEnable()
    {
        Event.OnPickupAbility += CollectAbility;
        Event.OnFinishRecharge += Recharge;
        Event.OnBatteryAdded += UpdateExtraCharge;
    }

    private void OnDisable()
    {
        Event.OnPickupAbility -= CollectAbility;
        Event.OnFinishRecharge -= Recharge;
    }

    private void Update()
    {
        // Decrease BatteryLife continuously over time based on Cost per second
        if (IsFlashlightOn && !IsBatteryDead())
            Drain(baseCost * Time.deltaTime);

        if (!IsBatteryDead()) return;

        if (CurrentAbility)
            CurrentAbility.OnStopAbility();

        if (!IsFlashlightOn || isFlickering) return;
        // Turn off the flashlight
        StartCoroutine(Flicker(1f, TurnOffLight));
        Event.SetTutorialText?.Invoke("Battery is Dead Press R to recharge"); //Ui to change battery
    }

    private void CollectAbility(FlashlightAbility ability)
    {
        if (flashlightAbilities.Contains(ability)) return;

        ability.Initialize(this);
        ability.gameObject.transform.parent = transform;
        ability.gameObject.transform.localPosition = Vector3.zero;
        flashlightAbilities.Add(ability);
        if (CurrentAbility == null)
            CurrentAbility = ability;
    }

    private void RemoveAbility(FlashlightAbility ability)
    {
        if (!flashlightAbilities.Contains(ability)) return;

        CurrentAbility = flashlightAbilities[0];
        flashlightAbilities.Remove(ability);
        Destroy(ability.gameObject); // Destroy the ability
    }

    public void HandleRayCast()
    {
        // Remove effects from objects that were affected in the last frame but are not in this frame
        for (var i = 0; i < effectedObjs.Count; i++)
        {
            if (effectedObjsThisFrame.Contains(effectedObjs[i])) continue;

            effectedObjs[i].RemoveEffect();
            effectedObjs.Remove(effectedObjs[i]);
        }

        if (IsBatteryDead() || !IsFlashlightOn) return;

        effectedObjsThisFrame.Clear();

        if (!Physics.Raycast(RayCastOrigin.position, RayCastOrigin.forward, out var hit, range)) return;

        var obj = hit.collider.gameObject;

        if (!obj.TryGetComponent(out IEffectable effectable)) return;

        // Try to get both IRevealable and IMovable components
        var hasRevealable = obj.TryGetComponent(out IRevealable revealObj); 
        var hashideable = obj.TryGetComponent(out IHideable hideObj);

        // Apply the reveal effect if not revealed && check if it has a movable component
        if (hasRevealable && revealObj.IsRevealed)
        {
            if (!hashideable)
            {
                return;
            }
        }

        if (!effectedObjs.Contains(effectable))
        {
            ApplyCurrentAbilityEffect(effectable);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Set the color for the Gizmos
        Gizmos.color = Color.red;

        // Ray and range definition
        var origin = player.PlayerCam.transform.position;
        var direction = player.PlayerCam.transform.forward * range;

        // Calculate the end point of the SphereCast
        var endPoint = origin + direction;

        // Draw the line representing the ray of the SphereCast
        Gizmos.DrawLine(origin, endPoint);
    }

    public void ResetLight(float cooldown)
    {
        // Reset the flashlight to its default state and ready for use
        StartCoroutine(ResetLightState(cooldown));
    }

    public void HandleAbility()
    {
        if (CurrentAbility != null && !IsBatteryDead() &&IsFlashlightOn && CanUseAbility)
        {
            CurrentAbility.OnUseAbility();
            CanUseAbility = false;
        }
        else
        {
            player.currentState?.HandleMove();
        }
    }

    public void HandleChangeAbility(int index)
    {
        var abilityNum = index - 1;
        if (IsFlashlightOn && abilityNum < flashlightAbilities.Count)
        {
            Debug.Log(abilityNum);
            CurrentAbility?.OnStopAbility();
            CurrentAbility = flashlightAbilities[abilityNum];
        }
        else
        {
            Debug.Log("No ability found in slot");
        }
    }

    public void StopUsingFlashlight()
    {
        if (CurrentAbility != null && IsFlashlightOn)
        {
            CurrentAbility.OnStopAbility();
        }
    }

    private IEnumerator ResetLightState(float cooldown)
    {
        CanUseAbility = false;
        Light.enabled = IsFlashlightOn = true;
        player.currentState?.HandleMove();

        var currentlIntensity = Light.intensity;
        var currentColor = Light.color;
        var currentRange = Light.range;
        var currentSpotAngle = Light.spotAngle;
        var currentInnerSpotAngle = Light.innerSpotAngle;

        var timer = 0f;
        while (timer < cooldown)
        {
            Light.intensity = Mathf.Lerp(currentlIntensity, intensity, timer / cooldown);
            Light.color = Color.Lerp(currentColor, lightColor, timer / cooldown);
            Light.range = Mathf.Lerp(currentRange, range, timer / cooldown);
            Light.spotAngle = Mathf.Lerp(currentSpotAngle, outerSpotAngle, timer / cooldown);
            Light.innerSpotAngle = Mathf.Lerp(currentInnerSpotAngle, innerSpotAngle, timer / cooldown);

            timer += Time.deltaTime;
            yield return null;
        }

        CanUseAbility = true;
    }

    private void TurnOffLight()
    {
        Light.enabled = IsFlashlightOn = false;

        //Remove effect on things
        for (var i = 0; i < effectedObjs.Count; i++)
        {
            effectedObjs[i].RemoveEffect();
            effectedObjs.Remove(effectedObjs[i]);
        }
    }

    public void ConsumeBattery(float cost)
    {
        if (!IsBatteryDead())
            Drain(cost);
    }

    private void TurnOnLight()
    {
        if (!IsBatteryDead())
        {
            ResetLight(1);
        }
        else
        {
            ResetLight(1);
            TurnOffLight();
        }
    }

    public void HandleFlashlightPower()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.FlashlightOnOff, transform.position);

        IsFlashlightOn = !IsFlashlightOn;
        if (IsFlashlightOn)
            TurnOnLight();
        else
            TurnOffLight();
    }

    private void ApplyCurrentAbilityEffect(IEffectable obj)
    {
        switch (obj)
        {
            case IRevealable revealable:
                if (CurrentAbility is RevealAbility)
                    obj.ApplyEffect();
                break;
            case IHideable hideable:
                if (CurrentAbility is DisapearAbility)
                    obj.ApplyEffect();
                break;
            default:
                obj.ApplyEffect();
                break;
        }

        effectedObjsThisFrame.Add(obj);
        effectedObjs.Add(obj);
    }

    private IEnumerator Flicker(float maxTime, Action onFlickerEnd)
    {
        isFlickering = true;
        var timer = 0f;
        while (timer < maxTime)
        {
            // Randomize the intensity
            Light.intensity = Mathf.PerlinNoise(0, intensity);

            // Randomize the time interval for the next flicker
            flickerTimer = Random.Range(minFlickerTime, maxFlickerTime);

            timer += Time.deltaTime;
            // Wait for the next flicker
            yield return new WaitForSeconds(flickerTimer);
        }

        isFlickering = false;

        onFlickerEnd?.Invoke();
    }

    private bool IsBatteryDead()
    {
        return BatteryLife <= 0;
    }

    private void Drain(float cost)
    {
        BatteryLife -= cost;
    }

    private void Recharge()
    {
        CurrentAbility?.OnStopAbility();
        BatteryLife = MaxBatteryLife + extraCharge;
        Event.SetTutorialTextTimer?.Invoke("Battery Recharged");
        TurnOnLight();
    }


    private void UpdateExtraCharge(float charge) => extraCharge = charge;

    public void ZeroOutBattery() => BatteryLife = 0;

    public IEnumerator ZeroOutLight(float cooldown, float zeroDownTime = 0.5f)
    {
        var delayTimer = 0f;

        // Store the initial properties of the flashlight
        var initialIntensity = Light.intensity;
        var flashlightColor = Light.color;
        var lightSpotAngle = Light.spotAngle;
        var initialInnerSpotAngle = Light.innerSpotAngle;


        while (delayTimer < zeroDownTime)
        {
            Light.intensity = Mathf.Lerp(initialIntensity, 0, delayTimer / zeroDownTime);
            Light.color = Color.Lerp(flashlightColor, Color.black, delayTimer / zeroDownTime);
            //Light.range = Mathf.Lerp(Light.range, 0, delayTimer / zeroDownTime);
            //Light.spotAngle = Mathf.Lerp(lightSpotAngle, 0, delayTimer / zeroDownTime);
            //Light.innerSpotAngle = Mathf.Lerp(initialInnerSpotAngle, 0f, delayTimer / zeroDownTime);

            delayTimer += Time.deltaTime;
            yield return null;
        }

        ResetLight(cooldown);

        CanUseAbility = true;
    }
}