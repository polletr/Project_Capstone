using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Flashlight.Ability;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

public class FlashLight : MonoBehaviour
{
    public GameEvent Event;
    public TutorialEvents TutorialEvent;

    // Battery Settings
    [field: Header("Battery Settings")]
    [field: SerializeField]
    public float BatteryLife { get; private set; }

    [field: SerializeField] public float MaxBatteryLife { get; private set; } = 100;

    [field: SerializeField] private float baseCost;

// Light Settings
    [field: Header("Light Settings")]
    [field: SerializeField]
    public Light Light { get; private set; }

    [field: SerializeField] public float InteractRange { get; protected set; }

    [field: SerializeField] public float CloseRangeInnerAngle { get; protected set; }
    [field: SerializeField] public float CloseRangeIntensity { get; protected set; }

    [SerializeField, Range(0, 2)] private float minCloseDistance;
    [SerializeField, Range(1, 10)] private float maxCloseDistance;

    [SerializeField] private float minFlickerTime = 0.1f;
    [SerializeField] private float maxFlickerTime = 0.5f;

// Base Flashlight Settings
    
    [field: Header("Base Flashlight Settings")]
    [field: SerializeField] public float BaseIntensity { get; protected set; }

    [field: SerializeField] public Color BaseColor { get; protected set; }

    [field: SerializeField] public float BaseSpotAngle { get; protected set; } = 60;

    [field: SerializeField] public float BaseInnerSpotAngle { get; protected set; } = 30;

// Build Up Flashlight Settings
    [field: Header("Build Up Flashlight Settings"), SerializeField]
    public float BuildupIntensity { get; protected set; }

    [field: SerializeField] public Color BuildupColor { get; protected set; }

    [field: SerializeField] public float BuildupSpotAngle { get; protected set; }

    [field: SerializeField] public float BuildupInnerSpotAngle { get; protected set; }

// Finish Flashlight Settings
    [field: Header("Finish Flashlight Settings"), SerializeField]
    public float FinalIntensity { get; protected set; }

    [field: SerializeField] public Color FinalColor { get; protected set; }

    [field: SerializeField] public float FinalSpotAngle { get; protected set; }

    [field: SerializeField] public float FinalInnerSpotAngle { get; protected set; }

// Flashlight Ability Bulb
    [Header("Flashlight Ability Bulb"), SerializeField]
    private Material disappearBulbMaterial;

    [SerializeField] private Material revealBulbMaterial;

    [SerializeField] private MeshRenderer flashlightBulb;

// Flashlight Abilities
    [SerializeField] private List<FlashlightAbility> flashlightAbilities;

// Delay Settings
    //[Header("Delay"), SerializeField] private float swapDelay = 0.5f;

// Miscellaneous Settings
    [field: SerializeField] public LayerMask IgnoreMask { get; private set; }

// Public Properties
    public float NewIntensity { get; set; } = 25;
    public float TotalBatteryLife => MaxBatteryLife + extraCharge;
    public Transform RayCastOrigin => player.PlayerCam.transform;
    private bool CanUseAbility { get; set; } = true;
    public bool IsFlashlightOn { get; private set; }
    public Vector3 FlashlightHitPos { get; set; }

// Private Fields
    private FlashlightAbility currentAbility;

    public FlashlightAbility CurrentAbility
    {
        get;
        set;
    }

    private float extraCharge;
    private float extracrank;

    private CountdownTimer flickerTimer;
    private CountdownTimer swapTimer;

    private PlayerController player;
    private GameObject effectedObject;

    private Coroutine flickerCoroutine;
    private Coroutine resetCoroutine;
    
    
    private void Awake()
    {
        flickerTimer = new CountdownTimer(1f);
        swapTimer = new CountdownTimer(0.5f);
        swapTimer.Start();
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

        Light.intensity  = NewIntensity = BaseIntensity ;
        Light.color = BaseColor;
        Light.spotAngle = BaseSpotAngle;
        Light.innerSpotAngle = BaseInnerSpotAngle;
    }

    private void OnEnable()
    {
        Event.OnFinishRecharge += Recharge;
        Event.OnBatteryAdded += UpdateExtraCharge;
        Event.OnCrankAdded += UpdateExtraCharge;
    }

    private void OnDisable()
    {
        Event.OnFinishRecharge -= Recharge;
    }

    private void Update()
    {
        flickerTimer?.Tick(Time.deltaTime);
        swapTimer?.Tick(Time.deltaTime);
        // Decrease BatteryLife continuously over time based on Cost per second
        if (IsFlashlightOn && !IsBatteryDead())
            Drain(baseCost * Time.deltaTime);

        if (!IsBatteryDead()) return;

        if (IsFlashlightOn && flickerCoroutine == null)
        {
            flickerCoroutine = StartCoroutine(Flicker(2f, TurnOffLight));
            Event.SetTutorialText?.Invoke("Press R to Recharge"); //Ui to change battery
        }
        // Turn off the flashlight
    }

    /// <summary>
    ///Based ont the hit point of the player set the intensity and angle of the light to the center of the screen unless we look down
    /// </summary>
    public void SetLightSettings(Vector3 hitPoint, bool center = true)
    {
        var newRotation = Quaternion.Euler(Vector3.zero);

        if (center)
        {
            Light.transform.localRotation = Quaternion.Slerp(Light.transform.localRotation, newRotation,
                player.Settings.flashlightFollowDelay * Time.deltaTime);
        }
        else
        {
            Light.transform.LookAt(this.transform.forward * 100);
        }


        // Calculate the intensity based on the distance
        var distance = Vector3.Distance(hitPoint, Light.transform.position);
        CheckDistance(distance);
        
    }
    
    private void CheckDistance(float distance)
    {
        var t = Mathf.InverseLerp(minCloseDistance, maxCloseDistance, distance);

        var targetIntensity = Mathf.Lerp(CloseRangeIntensity, NewIntensity, t);

        Light.intensity = Mathf.Lerp(Light.intensity, targetIntensity,20f * Time.deltaTime);
    }

    public void HandleRayCast()
    {
        if (IsBatteryDead() || !IsFlashlightOn)
        {
            if (effectedObject != null)
                RemoveCurrentAbilityEffect(effectedObject);

            StopUsingFlashlight();
            return;
        }

        if (Physics.Raycast(RayCastOrigin.position, RayCastOrigin.forward, out var hit, InteractRange))
        {
            var obj = hit.collider.gameObject;

            if (effectedObject != null)
            {
                if (Vector3.Distance(hit.point, transform.position) > InteractRange)
                {
                    RemoveCurrentAbilityEffect(effectedObject);
                    Debug.Log("Too Far");
                    return;
                }
            }

            if (!obj.TryGetComponent(out IEffectable effectable))
            {
                if (effectedObject != null)
                    RemoveCurrentAbilityEffect(effectedObject);
                return;
            }

            if (effectedObject != null && effectedObject != obj)
            {
                RemoveCurrentAbilityEffect(effectedObject);
            }

            effectedObject = obj;

/*            Ray ray = new Ray(RayCastOrigin.position, RayCastOrigin.forward * InteractRange);
            bool objectEffected = (effectedObject.GetComponent<Collider>().bounds.IntersectRay(ray));

*/

            ApplyCurrentAbilityEffect(effectedObject);

            FlashlightHitPos = hit.point;
            //Debug.Log($"{hit.collider.gameObject.name} was hit in {hit.collider.gameObject.scene.name} scene");
        }
        else
        {
            if (effectedObject != null)
                RemoveCurrentAbilityEffect(effectedObject);
            return;
        }


    }

    public void ResetLight(float cooldown)
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        // Reset the flashlight to its default state and ready for use
        resetCoroutine = StartCoroutine(ResetLightState(cooldown));
    }

    public void HandleAbility()
    {
        if (Physics.Raycast(RayCastOrigin.position, RayCastOrigin.forward, out var hit,
        InteractRange))
        {
            if (hit.collider.TryGetComponent(out DisappearObject obj) && obj.enabled)
            {
                if (flashlightAbilities.Any(a => a is DisappearAbility) && !IsBatteryDead() && IsFlashlightOn && CanUseAbility)
                {
                    DisappearAbility disappearAbility = flashlightAbilities.OfType<DisappearAbility>().FirstOrDefault();
                    if (disappearAbility != null)
                    {
                        disappearAbility.OnUseAbility();
                        CurrentAbility = disappearAbility;
                    }
                    CanUseAbility = false;
                }
                else
                {
                    player.currentState?.HandleMove();
                }
            }
            else if (hit.collider.TryGetComponent(out RevealableObject revealObj) && revealObj.enabled)
            {
                if (flashlightAbilities.Any(a => a is RevealAbility) && !IsBatteryDead() && IsFlashlightOn && CanUseAbility)
                {
                    RevealAbility revealAbility = flashlightAbilities.OfType<RevealAbility>().FirstOrDefault();
                    if (revealAbility != null)
                    {
                        revealAbility.OnUseAbility();
                        CurrentAbility = revealAbility;
                    }
                    CanUseAbility = false;
                }
                else
                {
                    player.currentState?.HandleMove();
                }
            }
            else
            {
                player.currentState?.HandleMove();
            }

        }
    }

    public void StopUsingFlashlight()
    {
        if (CurrentAbility != null)
        {
            CurrentAbility.OnStopAbility();
        }
    }

    private IEnumerator ResetLightState(float cooldown)
    {
        CanUseAbility = false;
        player.currentState?.HandleMove();

        var currentIntensity = Light.intensity;
        var currentColor = Light.color;
        var currentSpotAngle = Light.spotAngle;
        var currentInnerSpotAngle = Light.innerSpotAngle;

        var timer = 0f;
        while (timer <= cooldown)
        {
            timer += Time.deltaTime;

            NewIntensity = Mathf.Lerp(currentIntensity, BaseIntensity, timer / cooldown);
            Light.color = Color.Lerp(currentColor, BaseColor, timer / cooldown);
            Light.spotAngle = Mathf.Lerp(currentSpotAngle, BaseSpotAngle, timer / cooldown);
            Light.innerSpotAngle =
                Mathf.Lerp(currentInnerSpotAngle, BaseInnerSpotAngle, timer / cooldown);

            yield return null;
        }

        HandleRayCast();

        CanUseAbility = true;
    }

    private void TurnOffLight()
    {
        Light.enabled = IsFlashlightOn = false;
        flickerCoroutine = null;
        //Remove effect on things
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
        {
            TutorialEvent.OnTurnOnFlashlight?.Invoke();
            TurnOnLight();
        }
        else
        {
            TutorialEvent.OnTurnOffFlashlight?.Invoke();
            TurnOffLight();
        }
    }

    private void ApplyCurrentAbilityEffect(GameObject obj)
    {
        if (obj == null) return;

        // Try to get both IRevealable and IMovable components
        if (obj.TryGetComponent(out IEffectable effectableObj))
        {
            effectableObj.ApplyEffect();
        }
    }

    private void RemoveCurrentAbilityEffect(GameObject obj)
    {
        if (obj == null) return;

        CurrentAbility?.OnStopAbility();

        // Try to get both IRevealable and IMovable components
        if (obj.TryGetComponent(out IEffectable effectableObj))
        {
            effectableObj.RemoveEffect();
        }

        effectedObject = null;
    }

    private IEnumerator Flicker(float maxTime, Action onFlickerEnd)
    {
        flickerTimer.Reset(maxTime);
        flickerTimer.Start();

        while (!flickerTimer.IsFinished)
        {
            // Randomize the intensity
            NewIntensity = Mathf.PerlinNoise(0, BaseIntensity);

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
        Event.SetReloadTextTimer?.Invoke("");
        TurnOnLight();
    }

    private void UpdateExtraCharge(float charge) => extraCharge = charge;
    private void UpdateExtraCrank(float crank) => extraCharge = crank;

    public void ZeroOutBattery() => BatteryLife = 0;

    private bool HasAbility(FlashlightAbility ability)
    {
        var hasAbility = ability switch
        {
            RevealAbility reveal => false,
            DisappearAbility disappear => false,
            StunAbility stun => false,
            _ => true
        };
        return hasAbility;
    }


    public IEnumerator ZeroOutLight(float cooldown, float zeroDownTime = 0.5f)
    {
        CanUseAbility = false;
        var delayTimer = 0f;

        var initialIntensity = Light.intensity;
        var flashlightColor = Light.color;

        while (delayTimer < zeroDownTime)
        {
            NewIntensity = Mathf.Lerp(initialIntensity, 0, delayTimer / zeroDownTime);
            Light.color = Color.Lerp(flashlightColor, Color.black, delayTimer / zeroDownTime);

            delayTimer += Time.deltaTime;
            yield return null;
        }


        ResetLight(cooldown);
    }
}