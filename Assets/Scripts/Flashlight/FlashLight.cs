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

    [field: Header("Base Flashlight Settings")] [SerializeField]
    private Color lightColor;

    [field: Header("Battery Settings")] [SerializeField]
    private float baseCost;

    [SerializeField] private float minFlickerTime = 0.1f; // Minimum time between flickers
    [SerializeField] private float maxFlickerTime = 0.5f; // Maximum time between flickers

    [field: SerializeField] public float BatteryLife { get; private set; }

    [field: SerializeField] public float MaxBatteryLife { get; private set; } = 100;


    [SerializeField] private List<FlashlightAbility> flashlightAbilities;

    [field: SerializeField] public LayerMask IgrnoreMask { get; private set; }

    private FlashlightAbility _currentAbility;

    public FlashlightAbility CurrentAbility
    {
        get => _currentAbility;
        private set
        {
            _currentAbility = value;

            if (!CurrentAbility)
            {
                Debug.LogWarning("No ability found trying to set nothing as current ability");
            }
            else
            {
                if (ObjectPickupUIHandler.Instance && IsFlashlightOn)
                {
                    ObjectPickupUIHandler.Instance.PickedUpObject(CurrentAbility.AbilityPickupData, 0.3f);
                }

                CurrentAbility.SetLight(Light);
            }
        }
    }

    public float TotalBatteryLife => MaxBatteryLife + extraCharge;

    public Transform RayCastOrigin => player.PlayerCam.transform;
    public Light Light { get; private set; }
    public bool CanUseAbility { get; private set; } = true;
    public bool IsFlashlightOn { get; private set; }

    private float extraCharge;

    private CountdownTimer flickerTimer;
    private PlayerController player;

    private List<IEffectable> effectedObjs = new();
    private HashSet<IEffectable> effectedObjsThisFrame = new();

    private Coroutine flickerCoroutine;

    private void Awake()
    {
        Light = GetComponent<Light>();
        flickerTimer = new CountdownTimer(1f);
        IsFlashlightOn = Light.enabled;

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
        Light.intensity = CurrentAbility.BaseIntensity;
        Light.color = CurrentAbility.BaseColor;
        Light.spotAngle = CurrentAbility.BaseSpotAngle;
        Light.innerSpotAngle = CurrentAbility.BaseInnerSpotAngle;
        Light.range = CurrentAbility.BaseRange;
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
        flickerTimer?.Tick(Time.deltaTime);
        // Decrease BatteryLife continuously over time based on Cost per second
        if (IsFlashlightOn && !IsBatteryDead())
            Drain(baseCost * Time.deltaTime);

        if (!IsBatteryDead()) return;

        if (CurrentAbility)
            CurrentAbility.OnStopAbility();

        if (IsFlashlightOn && flickerCoroutine == null)
        {
            flickerCoroutine = StartCoroutine(Flicker(2f, TurnOffLight));
            Event.SetTutorialText?.Invoke("Battery is Dead Press R to recharge"); //Ui to change battery
        }
        // Turn off the flashlight
    }


    private void CollectAbility(FlashlightAbility ability)
    {
        if (flashlightAbilities.Contains(ability)) return;

        ability.Initialize(this);
        ability.gameObject.transform.parent = transform;
        ability.gameObject.transform.localPosition = Vector3.zero;
        flashlightAbilities.Add(ability);
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

        if (!Physics.Raycast(RayCastOrigin.position, RayCastOrigin.forward, out var hit, Light.range)) return;

        var obj = hit.collider.gameObject;

        if (!obj.TryGetComponent(out IEffectable effectable)) return;

        effectedObjsThisFrame.Add(effectable);

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
        var direction = player.PlayerCam.transform.forward * Light.range;

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
        if (CurrentAbility != null && !IsBatteryDead() && IsFlashlightOn && CanUseAbility)
        {
            CurrentAbility.OnUseAbility();
            CanUseAbility = false;
        }
        else
        {
            player.currentState?.HandleMove();
        }
    }

    public void HandleChangeAbility(int index, bool isScroll = false)
    {
        if (!IsFlashlightOn || !CurrentAbility || IsBatteryDead()) return;

        var currentIndex = flashlightAbilities.IndexOf(CurrentAbility);

        if (isScroll)
        {
            // determine scroll direction
            switch (index)
            {
                case > 0: // Scroll up
                    currentIndex = (currentIndex + 1) % flashlightAbilities.Count;
                    break;
                case < 0: // Scroll down
                    currentIndex = (currentIndex - 1 + flashlightAbilities.Count) % flashlightAbilities.Count;
                    break;
            }
        }
        else
        {
            // Direct selection, keyboard 1 , 2, 3 
            currentIndex = index - 1;

            if (currentIndex < 0 || currentIndex >= flashlightAbilities.Count)
            {
                Debug.Log("No ability found in slot");
                return;
            }
        }

        // Stop current ability and set the new one
        CurrentAbility = flashlightAbilities[currentIndex];
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
        player.currentState?.HandleMove();

        var currentlIntensity = Light.intensity;
        var currentColor = Light.color;
        var currentRange = Light.range;
        var currentSpotAngle = Light.spotAngle;
        var currentInnerSpotAngle = Light.innerSpotAngle;

        var timer = 0f;
        while (timer < cooldown)
        {
            Light.intensity = Mathf.Lerp(currentlIntensity, CurrentAbility.BaseIntensity, timer / cooldown);
            Light.color = Color.Lerp(currentColor, CurrentAbility.BaseColor, timer / cooldown);
            Light.range = Mathf.Lerp(currentRange, CurrentAbility.BaseRange, timer / cooldown);
            Light.spotAngle = Mathf.Lerp(currentSpotAngle, CurrentAbility.BaseSpotAngle, timer / cooldown);
            Light.innerSpotAngle =
                Mathf.Lerp(currentInnerSpotAngle, CurrentAbility.BaseInnerSpotAngle, timer / cooldown);

            timer += Time.deltaTime;
            yield return null;
        }

        CanUseAbility = true;
    }

    private void TurnOffLight()
    {
        Light.enabled = IsFlashlightOn = false;

        //Remove effect on things
        for (var i = effectedObjs.Count - 1; i >= 0; i--)
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
            Light.enabled = IsFlashlightOn = true;
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
        if (IsBatteryDead()) return;

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

        effectedObjs.Add(obj);
    }

    private IEnumerator Flicker(float maxTime, Action onFlickerEnd)
    {
        flickerTimer.Reset(maxTime);
        flickerTimer.Start();

        while (!flickerTimer.IsFinished)
        {
            // Randomize the intensity
            Light.intensity = Mathf.PerlinNoise(0, CurrentAbility.BaseIntensity);

            // Randomize the time interval for the next flicker
            var time = Random.Range(minFlickerTime, maxFlickerTime);

            // Wait for the next flicker
            yield return new WaitForSeconds(time);
        }

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
        if (flickerCoroutine != null)
            StopCoroutine(flickerCoroutine);

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