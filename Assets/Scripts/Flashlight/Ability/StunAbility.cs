using System;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class StunAbility : FlashlightAbility
{
  
    
    [SerializeField] private Color flashColor;
    [SerializeField] private float flashIntensity;
    [SerializeField] private float flashspotAngle;
    [SerializeField] private float flashInnerSpotAngle;
    
    [SerializeField] private float effectRadius;
    [SerializeField] private float buildUpTime;

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
        Flashlight.Light.intensity = flashIntensity;
        Flashlight.Light.color = flashColor;
        Flashlight.Light.spotAngle = flashspotAngle;
        Flashlight.Light.innerSpotAngle = flashInnerSpotAngle;

        if (!PlayerBatteryUIHandler.Instance)
            PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();

        var ray = new Ray(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward * Flashlight.Light.range);

        var hits = new RaycastHit[10];
        var size = Physics.SphereCastNonAlloc(ray, effectRadius, hits, Flashlight.Light.range, Flashlight.IgrnoreMask);

        for (var i = 0; i < size; i++)
        {
            var hit = hits[i];
            if (!hit.collider) continue;
            var obj = hit.collider.gameObject;

            if (obj.TryGetComponent(out IStunable thing))
                thing.ApplyStunEffect();
        }

        flashSound = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.FlashlightStun);
        flashSound.start();
        Flashlight.ConsumeBattery(Cost);

        
        //set light to stun flash properties 
        Flashlight.StartCoroutine(Flashlight.ZeroOutLight(Cooldown));
        
    }
    
    private IEnumerator StartStunAttack()
    {
        var delayTimer = 0f;

        // Store the initial properties of the flashlight
        var initialIntensity = Flashlight.Light.intensity;
        var flashlightColor = Flashlight.Light.color;
        var lightSpotAngle = Flashlight.Light.spotAngle;
        var initialInnerSpotAngle = Flashlight.Light.innerSpotAngle;


        while (delayTimer < buildUpTime)
        {
            Flashlight.Light.intensity = Mathf.Lerp(initialIntensity, 50, delayTimer / buildUpTime);
            Flashlight.Light.color = Color.Lerp(flashlightColor, Color.red, delayTimer / buildUpTime);
            Flashlight.Light.spotAngle = Mathf.Lerp(lightSpotAngle, 5f, delayTimer / buildUpTime);
            Flashlight.Light.innerSpotAngle = Mathf.Lerp(initialInnerSpotAngle, 0f, delayTimer / buildUpTime);

            delayTimer += Time.deltaTime;
            yield return null;
        }
        
        Stun();
    }
}