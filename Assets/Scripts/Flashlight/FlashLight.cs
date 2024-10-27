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
    private float range;

    [SerializeField] private Color lightColor;
    [SerializeField] private float intensity;

    [SerializeField] private float baseCost;
    [SerializeField] private float minBatteryAfterUse;

    [SerializeField] private float minFlickerTime = 0.1f; // Minimum time between flickers
    [SerializeField] private float maxFlickerTime = 0.5f; // Maximum time between flickers

    [field: SerializeField] public float BatteryLife { get; private set; }


    [field: SerializeField] public float MaxBatteryLife { get; private set; } = 100;

    public float TotalBatteryLife => MaxBatteryLife + extraCharge;

    [SerializeField] private List<FlashlightAbility> flashlightAbilities;


    public PlayerController Player { get; private set; }

    public Transform RayCastOrigin => Player.PlayerCam.transform;
    public FlashlightAbility CurrentAbility { get; private set; }

    public Light Light { get; private set; }

    [field: SerializeField] public LayerMask IgrnoreMask { get; private set; }

    public bool IsFlashlightOn { get; private set; }

    private float flickerTimer;
    private float extraCharge;

    private bool isFlickering;

    private List<IEffectable> effectedObjs = new();
    private HashSet<IEffectable> effectedObjsThisFrame = new();

    private CountdownTimer cooldownTimer;

    private void Awake()
    {
        cooldownTimer = new CountdownTimer(0.1f);
        cooldownTimer.Start();

        Light = GetComponent<Light>();
        IsFlashlightOn = Light.enabled;
        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;

        Player = GetComponentInParent<PlayerController>();

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
        cooldownTimer?.Tick(Time.deltaTime);
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
        // Apply the reveal effect if not revealed && check if it has a movable component
        if (hasRevealable && revealObj.IsRevealed) return;

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
        var origin = Player.PlayerCam.transform.position;
        var direction = Player.PlayerCam.transform.forward * range;

        // Calculate the end point of the SphereCast
        var endPoint = origin + direction;

        // Draw the line representing the ray of the SphereCast
        Gizmos.DrawLine(origin, endPoint);
    }

    public void ResetLight()
    {
        // Reset the flashlight to its default state and ready for use
        ResetLightState();
    }

    public void HandleFlashAbility()
    {
        if (CurrentAbility != null && IsFlashlightOn && BatteryLife - CurrentAbility.Cost > minBatteryAfterUse)
            CurrentAbility.OnUseAbility();
        else
            Player.currentState?.HandleMove();
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
            CurrentAbility.OnStopAbility();
    }

    private void ResetLightState()
    {
        Light.enabled = IsFlashlightOn = true;

        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;
        Player.currentState?.HandleMove();
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
            ResetLightState();
        }
        else
        {
            ResetLightState();
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
        obj.ApplyEffect();
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
            Light.intensity = Mathf.PerlinNoise(0 , intensity);

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
        BatteryLife = MaxBatteryLife + extraCharge;
        Event.SetTutorialTextTimer?.Invoke("Battery Recharged");
        TurnOnLight();
    }

    private void UpdateExtraCharge(float charge) => extraCharge = charge;

    public void ZeroOutBattery() => BatteryLife = 0;
}