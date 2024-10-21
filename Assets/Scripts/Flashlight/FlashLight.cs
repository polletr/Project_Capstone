using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    [field: SerializeField] public Transform MoveHoldPos { get; private set; }

    [SerializeField] private List<FlashlightAbility> flashlightAbilities;


    public PlayerController Player { get; private set; }
    
    public Transform RayCastOrigin => Player.PlayerCam.transform;
    public FlashlightAbility CurrentAbility { get; private set; }

    public Light Light { get; private set; }

    [SerializeField] private LayerMask layerMask;

    private float flickerTimer;
    private float _extraCharge;

    private bool isFlashlightOn;
    private bool isFlickering;

    private List<IEffectable> effectedObjs = new();
    private HashSet<IEffectable> effectedObjsThisFrame = new();


    private void Awake()
    {
        Light = GetComponent<Light>();
        isFlashlightOn = Light.enabled;
        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;

        Player = GetComponentInParent<PlayerController>();

        MoveHoldPos = MoveHoldPos == null ? transform : MoveHoldPos;

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
        if (isFlashlightOn && !IsBatteryDead())
            Drain(baseCost * Time.deltaTime);

        if (!IsBatteryDead()) return;

        if (CurrentAbility != null)
            CurrentAbility.OnStopAbility();

        if (isFlashlightOn && !isFlickering)
        {
            // Turn off the flashlight
            StartCoroutine(Flicker(1f, TurnOffLight));
            Event.SetTutorialText?.Invoke("Battery is Dead Press Q to recharge"); //Ui to change battery

        }
    }

    private void CollectAbility(FlashlightAbility ability)
    {
        // check if its in flashlightAbilities
        // add and enable it
        if (flashlightAbilities.Contains(ability)) return;

        ability.Initialize(this);
        ability.gameObject.transform.parent = transform;
        flashlightAbilities.Add(ability);
    }

    private void RemoveAbility(FlashlightAbility ability)
    {
        if (!flashlightAbilities.Contains(ability)) return;

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
        
        if (IsBatteryDead() || !isFlashlightOn) return;
      
        effectedObjsThisFrame.Clear();

        if (!Physics.Raycast(RayCastOrigin.position, RayCastOrigin.forward, out var hit, range)) return;
        
        var obj = hit.collider.gameObject;

        if (!obj.TryGetComponent(out IEffectable effectable)) return;

        // Try to get both IRevealable and IMovable components
        var hasRevealable = obj.TryGetComponent(out IRevealable revealObj);
        var hasMovable = obj.TryGetComponent(out IMovable movableObj);
        
     // Apply the reveal effect if not revealed && check if it has a movable component
        if (hasRevealable)
        {
            if (!revealObj.IsRevealed)
            {
                revealObj.ApplyEffect();
                effectedObjs.Add(revealObj);
                effectedObjsThisFrame.Add(revealObj);
            }
            else if (hasMovable)
            {
                var distance =((MoveAbility)flashlightAbilities.Find(ability => ability is MoveAbility)).PickupRange;

                if (!(Vector3.Distance(RayCastOrigin.position, obj.transform.position) <= distance)) return;
                
                movableObj.ApplyEffect();
                effectedObjs.Add(movableObj);
                effectedObjsThisFrame.Add(movableObj);
            }
        }
        else
        {
            // If it's IEffectable and hasn't been affected yet, apply its effect
            effectedObjsThisFrame.Add(effectable);
            if (!effectedObjs.Contains(effectable))
            {
                ApplyCurrentAbilityEffect(obj);
            }
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
        ResetLightState();
    }

    public void HandleFlashAbility()
    {
        if (Physics.Raycast(Player.PlayerCam.transform.position, Player.PlayerCam.transform.forward, out var hit,
                range))
            CurrentAbility = hit.collider.TryGetComponent(out RevealableObject obj) && !obj.IsRevealed
                ? flashlightAbilities.Find(ability => ability is RevealAbility)
                : flashlightAbilities.Find(ability => ability is MoveAbility);


        if (CurrentAbility != null && isFlashlightOn && (BatteryLife - CurrentAbility.Cost) >= minBatteryAfterUse)
            CurrentAbility.OnUseAbility();
        else
            Player.currentState?.HandleMove();
    }

    public void HandleStunAbility()
    {
        CurrentAbility = flashlightAbilities.Find(ability => ability is StunAbility);
        if (CurrentAbility != null && isFlashlightOn && (BatteryLife - CurrentAbility.Cost) >= minBatteryAfterUse)
            CurrentAbility.OnUseAbility();
        else
            Player.currentState?.HandleMove();
    }


    public void StopUsingFlashlight()
    {
        if (CurrentAbility != null && isFlashlightOn)
            CurrentAbility.OnStopAbility();
    }

    private void ResetLightState()
    {
        Light.enabled = true;
        isFlashlightOn = true;
        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;
        Player.currentState?.HandleMove();
    }

    private void TurnOffLight()
    {
        Light.enabled = false;
        isFlashlightOn = false;
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
        else
            Event.SetTutorialText?.Invoke("Battery is Dead Press Q to recharge"); //Ui to change battery
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

        isFlashlightOn = !isFlashlightOn;
        if (isFlashlightOn)
            TurnOnLight();
        else
            TurnOffLight();
    }


    private void ApplyCurrentAbilityEffect(GameObject obj)
    {
        if (!obj.TryGetComponent(out IEffectable effectable)) return;

        effectable.ApplyEffect();
        effectedObjsThisFrame.Add(effectable);
        effectedObjs.Add(effectable);
    }

    private IEnumerator Flicker(float maxTime, Action onFlickerEnd)
    {
        isFlickering = true;
        var timer = 0f;
        while (timer < maxTime)
        {
            Debug.Log("Flickering");
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
        BatteryLife = MaxBatteryLife + _extraCharge;
        Event.SetTutorialTextTimer?.Invoke("Battery Recharged");
    }

    private void UpdateExtraCharge(float charge) => _extraCharge = charge;

    public void ZeroOutBattery() => BatteryLife = 0;
}