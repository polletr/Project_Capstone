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


    [Header("Base Flashlight Settings"), SerializeField]
    private Color lightColor;

    [field: Header("Battery Settings"), SerializeField]
    private float baseCost;

    [SerializeField] private float minFlickerTime = 0.1f; // Minimum time between flickers
    [SerializeField] private float maxFlickerTime = 0.5f; // Maximum time between flickers

    [field: SerializeField] public float BatteryLife { get; private set; }

    [field: SerializeField] public float MaxBatteryLife { get; private set; } = 100;

    [field: SerializeField, Header("Light Settings")]
    public Light Light { get; private set; }

    [SerializeField, Range(0, 2)] private float minCloseDistance;
    [SerializeField, Range(1, 3)] private float maxCloseDistance;

    [SerializeField] private List<FlashlightAbility> flashlightAbilities;

    [field: SerializeField] public LayerMask IgrnoreMask { get; private set; }

    [Header("flashlight Ability Bulb")] [SerializeField]
    private Material disappearBulbMaterial;
    [SerializeField] private Material revealBulbMaterial;
    [SerializeField] private MeshRenderer flashlightBulb;

    [Header("Delay "), SerializeField] private float swapDelay = 0.5f;

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
                CurrentAbility.SetLight(Light);
            }
        }
    }

    public float TotalBatteryLife => MaxBatteryLife + extraCharge;
    public float CrankBoost => extracrank;


    public Transform RayCastOrigin => player.PlayerCam.transform;
    public bool CanUseAbility { get; private set; } = true;
    public bool IsFlashlightOn { get; private set; }
    public Vector3 FlaslighHitPos { get; set; }

    private float extraCharge;
    private float extracrank;

    private CountdownTimer flickerTimer;
    private CountdownTimer swapTimer;
    private PlayerController player;

    private List<IEffectable> effectedObjs = new();
    private HashSet<IEffectable> effectedObjsThisFrame = new();

    private Coroutine flickerCoroutine;
    private Coroutine resetCoroutine;

    private void Awake()
    {
        // Light = GetComponent<Light>();
        //Light = GetComponentInChildren<Light>();
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

        CurrentAbility = flashlightAbilities[0];
        ChangeMaterial();
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
        Event.OnCrankAdded += UpdateExtraCharge;
    }

    private void OnDisable()
    {
        Event.OnPickupAbility -= CollectAbility;
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

        if (CurrentAbility)
            CurrentAbility.OnStopAbility();

        if (IsFlashlightOn && flickerCoroutine == null)
        {
            flickerCoroutine = StartCoroutine(Flicker(2f, TurnOffLight));
            Event.SetTutorialText?.Invoke("Battery is Dead Press R to recharge"); //Ui to change battery
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
        var distance = Vector3.Distance(hitPoint, transform.position);
        if (distance > minCloseDistance)
        {
            //Debug.Log("very far");
            Light.intensity = CurrentAbility.BaseIntensity;
        }
        else
        {
            //Debug.Log("very close");
            Light.intensity = CurrentAbility.CloseRangeIntensity;
        }
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
            if (effectedObjsThisFrame.Contains(effectedObjs[i]))
            {
                RemoveCurrentAbilityEffect(effectedObjs[i]);
                continue;
            }

            effectedObjs[i].RemoveEffect();
            effectedObjs.Remove(effectedObjs[i]);
        }

        if (IsBatteryDead() || !IsFlashlightOn) return;

        effectedObjsThisFrame.Clear();

        if (Physics.Raycast(RayCastOrigin.position, RayCastOrigin.forward, out var hit))
        {
            FlaslighHitPos = hit.point;
            if (Vector3.Distance(hit.point, transform.position) > CurrentAbility.InteractRange)
            {
                return;
            }
        }
        else
        {
            return;
        }

        var obj = hit.collider.gameObject;

        //CheckDistance(hit.distance);

        if (!obj.TryGetComponent(out IEffectable effectable)) return;
        effectedObjsThisFrame.Add(effectable);

        var hasRevealable = obj.TryGetComponent(out IRevealable revealObj);
        var hashideable = obj.TryGetComponent(out IHideable hideObj);

        // Check if revealObj is enabled
        if (hasRevealable && revealObj is MonoBehaviour revealComponent && revealComponent.enabled)
        {
            if (!revealObj.CanApplyEffect)
                return;
        }

        // Check if revealObj is enabled
        if (hashideable && hideObj is MonoBehaviour hideComponent && hideComponent.enabled)
        {
            if (!hideObj.CanApplyEffect)
                return;
        }

        // Apply the reveal effect if not revealed && check if it has a hidable component
        if (hasRevealable && revealObj.IsRevealed)
        {
            if (!hashideable)
            {
                return;
            }
        }

        if (!effectedObjs.Contains(effectable))
        {
            ApplyCurrentAbilityEffect(obj);
        }
    }

    private void CheckDistance(float distance)
    {
        // Calculate intensity based on distance
        float intensity = Mathf.Lerp(CurrentAbility.CloseRangeIntensity, CurrentAbility.BaseIntensity,
            distance / minCloseDistance);
        Light.intensity = Mathf.Clamp(intensity, CurrentAbility.CloseRangeIntensity, CurrentAbility.BaseIntensity);
        Debug.Log(Light.intensity);

        // Calculate inner angle based on distance
        float angle = Mathf.Lerp(CurrentAbility.CloseRangeInnerAngle, CurrentAbility.BaseInnerSpotAngle,
            distance / minCloseDistance);
        Light.spotAngle = Mathf.Clamp(angle, CurrentAbility.CloseRangeInnerAngle, CurrentAbility.BaseInnerSpotAngle);
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
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        // Reset the flashlight to its default state and ready for use
        resetCoroutine = StartCoroutine(ResetLightState(cooldown));
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
        if (!IsFlashlightOn || !CurrentAbility || IsBatteryDead() || !swapTimer.IsFinished) return;


        var currentIndex = flashlightAbilities.IndexOf(CurrentAbility);

        if (flashlightAbilities.Count <= 1)
        {
            return;
        }

        if (isScroll)
        {
            // determine scroll direction
            swapTimer.Reset(swapDelay);
            swapTimer.Start();
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
            else if (currentIndex == flashlightAbilities.IndexOf(CurrentAbility))
            {
                return;
            }
        }

        // Stop current ability and set the new one
        CurrentAbility = flashlightAbilities[currentIndex];
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.FlashlightSwapAbility,
            transform.position);
        ChangeMaterial();
        TutorialEvent.OnSwapAbility?.Invoke();

        if (ObjectPickupUIHandler.Instance != null)
        {
            ObjectPickupUIHandler.Instance.PickedUpObject(CurrentAbility.AbilityPickupData, 0.3f);
        }

        foreach (var t in effectedObjs)
        {
            RemoveCurrentAbilityEffect(t);
        }
    }

    private void ChangeMaterial()
    {
        switch (CurrentAbility)
        {
            case RevealAbility:
                flashlightBulb.material = revealBulbMaterial;
                break;
            case DisappearAbility:
                flashlightBulb.material = disappearBulbMaterial;
                break;
            /*case StunAbility:
                flashlightBulb.material = stunBulbMaterial;
                break;*/
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
        player.currentState?.HandleMove();

        var currentIntensity = Light.intensity;
        var currentColor = Light.color;
        var currentRange = Light.range;
        var currentSpotAngle = Light.spotAngle;
        var currentInnerSpotAngle = Light.innerSpotAngle;

        var timer = 0f;
        while (timer < cooldown)
        {
            Light.intensity = Mathf.Lerp(currentIntensity, CurrentAbility.BaseIntensity, timer / cooldown);
            Light.color = Color.Lerp(currentColor, CurrentAbility.BaseColor, timer / cooldown);
            Light.range = Mathf.Lerp(currentRange, CurrentAbility.BaseRange, timer / cooldown);
            Light.spotAngle = Mathf.Lerp(currentSpotAngle, CurrentAbility.BaseSpotAngle, timer / cooldown);
            Light.innerSpotAngle =
                Mathf.Lerp(currentInnerSpotAngle, CurrentAbility.BaseInnerSpotAngle, timer / cooldown);

            timer += Time.deltaTime;
            yield return null;
        }

        effectedObjs.Clear();
        HandleRayCast();

        CanUseAbility = true;
        // StopAllCoroutines();
    }

    private void TurnOffLight()
    {
        Light.enabled = IsFlashlightOn = false;
        flickerCoroutine = null;
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
        // Try to get both IRevealable and IMovable components
        var hasRevealable = obj.TryGetComponent(out IRevealable revealObj);
        var hashideable = obj.TryGetComponent(out IHideable hideObj);
        var hasStunable = obj.TryGetComponent(out IStunable stunObj);

        // Check if the object is IRevealable and CurrentAbility is RevealAbility
        if (revealObj is MonoBehaviour revealComponent && revealComponent.enabled)
        {
            if (CurrentAbility is RevealAbility)
            {
                revealObj.ApplyEffect();
                effectedObjs.Add(revealObj);
            }
        }

        // Check if the object is IHideable and CurrentAbility is DisapearAbility
        else if (hideObj is MonoBehaviour hideComponent && hideComponent.enabled)
        {
            if (CurrentAbility is DisappearAbility)
            {
                hideObj.ApplyEffect();
                effectedObjs.Add(hideObj);
            }
        }

        // Check if the object is IStunable and CurrentAbility is StunAbility
        else if (stunObj is IStunable stunable)
        {
            if (CurrentAbility is StunAbility)
            {
                stunObj.ApplyEffect();
                effectedObjs.Add(stunObj);
            }
        }
        else if(obj.TryGetComponent(out IEffectable effectableObj))
        {
            effectableObj.ApplyEffect();
            effectedObjs.Add(effectableObj);
        }
    }

    private void RemoveCurrentAbilityEffect(IEffectable obj)
    {
        if (obj == null) return;

        obj.RemoveEffect();
        effectedObjs.Remove(obj);

/*        // Check if the object is IRevealable and the ability is not RevealAbility
        if (obj is IRevealable && CurrentAbility is not RevealAbility)
        {
            obj.RemoveEffect();
            effectedObjs.Remove(obj);
        }

        // Check if the object is IHideable and the ability is not DisapearAbility
        else if (obj is IHideable && CurrentAbility is not DisappearAbility)
        {
            obj.RemoveEffect();
            effectedObjs.Remove(obj);
        }
        // Check if the object is IHideable and the ability is not DisapearAbility
        else if (obj is IStunable && CurrentAbility is not StunAbility)
        {
            obj.RemoveEffect();
            effectedObjs.Remove(obj);
        }
*/    }

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
        Event.SetReloadTextTimer?.Invoke("");
        TurnOnLight();
    }


    private void UpdateExtraCharge(float charge) => extraCharge = charge;
    private void UpdateExtraCrank(float crank) => extraCharge = crank;

    public void ZeroOutBattery() => BatteryLife = 0;

    public IEnumerator ZeroOutLight(float cooldown, float zeroDownTime = 0.5f)
    {
        CanUseAbility = false;
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
    }
}