using FMOD.Studio;
using System.Collections;
using UnityEngine;

public class StunAbility : FlashlightAbility
{
    [field: Header("Stun Ability Settings")]
    [SerializeField] private float effectRadius;

    [Header("Build Up Properties")]
    [SerializeField] private Color buildUpColor;
    [SerializeField] private float buildUpIntensity;
    [SerializeField] private float buildUpSpotAngle;
    [SerializeField] private float buildUpInnerSpotAngle;

    private EventInstance flashSound;

    public override void OnUseAbility()
    {
        StartCoroutine(StartStunAttack());
    }


    public override void OnStopAbility()
    {
        StopAllCoroutines();
        Flashlight.ResetLight(0.5f);
    }

    private void Stun()
    {


//set light to stun flash properties 
        Flashlight.Light.intensity = AbilityIntensity;
        Flashlight.Light.color = AbilityColor;
        Flashlight.Light.spotAngle = AbilitySpotAngle;
        Flashlight.Light.innerSpotAngle = AbilityInnerSpotAngle;

        if (!PlayerBatteryUIHandler.Instance)
            PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();

        var ray = new Ray(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, effectRadius, InteractRange);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                var obj = hit.collider.gameObject;
                Debug.Log("Hit object: " + obj.name);

                if (obj.TryGetComponent(out IStunable thing))
                {
                    thing.ApplyStunEffect();
                    Debug.Log("Apply Stun from Flashlight on " + obj.name);
                }
            }
        }
        flashSound = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.FlashlightStun);
        flashSound.start();
        Flashlight.ConsumeBattery(Cost);

        
        //set light to stun flash properties 
        Flashlight.StartCoroutine(Flashlight.ZeroOutLight(Cooldown));
        PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();
        
        tutorialEvents.OnStun?.Invoke();
    }
    
    private IEnumerator StartStunAttack()
    {
        var timer = 0f;

        // Store the initial properties of the flashlight
        var initialIntensity = Flashlight.Light.intensity;
        var flashlightColor = Flashlight.Light.color;
        var lightSpotAngle = Flashlight.Light.spotAngle;
        var initialInnerSpotAngle = Flashlight.Light.innerSpotAngle;


        while (timer < AbilityBuildUpTime)
        {
            Flashlight.Light.intensity = Mathf.Lerp(initialIntensity, buildUpIntensity, timer / AbilityBuildUpTime);
            Flashlight.Light.color = Color.Lerp(flashlightColor, buildUpColor, timer / AbilityBuildUpTime);
            Flashlight.Light.spotAngle = Mathf.Lerp(lightSpotAngle, buildUpSpotAngle, timer / AbilityBuildUpTime);
            Flashlight.Light.innerSpotAngle = Mathf.Lerp(initialInnerSpotAngle, buildUpInnerSpotAngle, timer / AbilityBuildUpTime);

            timer += Time.deltaTime;
            yield return null;
        }
        
        Stun();
    }
}