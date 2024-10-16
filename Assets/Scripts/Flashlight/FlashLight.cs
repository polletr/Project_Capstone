using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class FlashLight : MonoBehaviour
{
    public GameEvent Event;

    [Header("Base Flashlight Settings")]
    [SerializeField] private float range;
    [SerializeField] private Color lightColor;
    [SerializeField] private float intensity;

    [SerializeField] private float baseCost;
    [SerializeField] private float minBatteryAfterUse;

    [SerializeField] private float minFlickerTime = 0.1f;  // Minimum time between flickers
    [SerializeField] private float maxFlickerTime = 0.5f;  // Maximum time between flickers

    [field: SerializeField] public float BatteryLife { get; private set; }

    [SerializeField] private List<FlashlightAbility> flashlightAbilities;

    public float MaxBatteryLife { get; private set; } = 100;

    public PlayerController Player { get; private set; }

    public FlashlightAbility CurrentAbility { get; private set; }

    public Light Light { get; set; }


    private float flickerTimer;
    private float _extraCharge;

    private bool isFlashlightOn;
    private bool isFlickering;

    private List<IEffectable> effectedObjs = new();
    private HashSet<IEffectable> effectedObjsThisFrame = new();

    private LayerMask layerMask;

    private void Awake()
    {
        Light = GetComponent<Light>();
        Light.enabled = true;
        isFlashlightOn = true;
        Light.range = range;
        Light.intensity = intensity;
        Light.color = lightColor;

        layerMask = LayerMask.GetMask("Flashlight");

        Player = GetComponentInParent<PlayerController>();


        if (flashlightAbilities.Count > 0)
        {
            CurrentAbility = flashlightAbilities[0];

            foreach (FlashlightAbility ability in flashlightAbilities)
            {
                if (ability != null)
                    ability.Initialize(this);
            }
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

        if (IsBatteryDead())
        {
            if (CurrentAbility != null)
                CurrentAbility.OnStopAbility();
            // Turn off the flashlight
            if (isFlashlightOn && !isFlickering)
                StartCoroutine(Flicker(3f, () => TurnOffLight()));
        }
    }

    private void CollectAbility(FlashlightAbility ability)
    {

        // check if its in flashlightAbilities
        // add and enable it
        if (!flashlightAbilities.Contains(ability))
        {
            ability.Initialize(this);
            ability.gameObject.transform.parent = transform;
            flashlightAbilities.Add(ability);

            if (flashlightAbilities.Count == 1)
                CurrentAbility = ability;
        }
    }

    private void RemoveAbility(FlashlightAbility ability)
    {
        if (flashlightAbilities.Contains(ability))
        {
            if (flashlightAbilities.Count <= 0)
                CurrentAbility = null;
            else
                CurrentAbility = flashlightAbilities[0];

            flashlightAbilities.Remove(ability);
            Destroy(ability.gameObject); // Destroy the ability
        }
    }

    public void HandleSphereCast()
    {
        if (IsBatteryDead() || !isFlashlightOn) return;

        Ray ray = new Ray(transform.position, transform.forward * range);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 1f, range);
        effectedObjsThisFrame.Clear();
        foreach (RaycastHit hit in hits)
        {
            var obj = hit.collider.gameObject;

            if (obj.TryGetComponent(out IEffectable effectable))
            {
                effectedObjsThisFrame.Add(effectable);
                if (!effectedObjs.Contains(effectable))
                {
                    ApplyCurrentAbilityEffect(obj);
                }
            }
        }

        // Remove effects from objects that were affected in the last frame but are not in this frame
        for (int i = 0; i < effectedObjs.Count; i++)
        {
            if (!effectedObjsThisFrame.Contains(effectedObjs[i]))
            {
                effectedObjs[i].RemoveEffect();
                effectedObjs.Remove(effectedObjs[i]);
            }
        }

    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Set the color for the Gizmos
        Gizmos.color = Color.red;

        // Ray and range definition
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward * range;

        // Draw the sphere at the origin point
        Gizmos.DrawWireSphere(origin, 2f);

        // Calculate the end point of the SphereCast
        Vector3 endPoint = origin + direction;

        // Draw the line representing the ray of the SphereCast
        Gizmos.DrawLine(origin, endPoint);

        // Draw the sphere at the end point
        Gizmos.DrawWireSphere(endPoint, 2f);
    }

    public void ResetLight()
    {
        if (this.gameObject.activeSelf)
            StartCoroutine(Flicker(1f, () => ResetLightState()));

    }

    public void HandleFlashAblility()
    {
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

    public void TurnOffLight()
    {

        Light.enabled = false;
        isFlashlightOn = false;
    }

    public void ConsumeBattery(float cost)
    {
        if (!IsBatteryDead())
            Drain(cost);
        else
            Debug.Log("Battery is Dead recharge B***H");//Ui to change battery
    }

    public void TurnOnLight()
    {
        if (!IsBatteryDead())
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
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.FlashlightOnOff, transform.position);

        isFlashlightOn = !isFlashlightOn;
        if (isFlashlightOn)
            TurnOnLight();
        else
            TurnOffLight();
    }


    public void ChangeSelectedAbility(int direction) // swap between abilities with mouse wheel
    {
        if (flashlightAbilities.Count() > 1)
        {
            int currentIndex = flashlightAbilities.IndexOf(CurrentAbility);

            currentIndex += direction;
            if (currentIndex >= flashlightAbilities.Count)
                currentIndex = 0;
            else if (currentIndex < 0)
                currentIndex = flashlightAbilities.Count - 1;

            CurrentAbility = flashlightAbilities[currentIndex];
        }
    }

    private void ApplyCurrentAbilityEffect(GameObject obj)
    {
        if (obj.TryGetComponent(out IStunnable enemy))
        {
            enemy.ApplyEffect();
            effectedObjs.Add(enemy);
        }

        if (obj.TryGetComponent(out IMovable moveObj))
        {
            moveObj.ApplyEffect();
            effectedObjs.Add(moveObj);
        }

        switch (CurrentAbility)
        {
            case RevealAbility revealAbility:
                if (obj.TryGetComponent(out IRevealable revealObj))
                {
                    revealObj.ApplyEffect();
                    effectedObjs.Add(revealObj);
                }
                break;
            case StunAbility stunAbility:
                if (obj.TryGetComponent(out IStunnable stunObj))
                {
                    stunObj.ApplyEffect();
                    effectedObjs.Add(stunObj);
                }
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
    }

    private void UpdateExtraCharge(float charge)
    {
        _extraCharge = charge;
    }

    public void ZeroOutBattery()
    {
        BatteryLife = 0;
    }

}
