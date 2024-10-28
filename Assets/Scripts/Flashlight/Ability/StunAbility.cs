using System;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class StunAbility : FlashlightAbility
{
    [SerializeField] private Color flashColor;
    [SerializeField] private Color cooldownColor;
    [SerializeField] private float decreaseRate;
    [SerializeField] private float effectRadius;
    [SerializeField] private float lightIntensity;
    [SerializeField] private float cooldownIntensity;
    [SerializeField] private float buildUpTime;

    private CountdownTimer timer;
    private EventInstance flashSound;

    private void Update() => timer?.Tick(Time.deltaTime);

    public override void OnUseAbility()
    {
        StartCoroutine(StartStunAttack());
    }


    public override void OnStopAbility()
    {
        StopAllCoroutines();
        Flashlight.StartCooldown(Cooldown);
        StartCoroutine(RestoreLightOverTime());
    }

    private void Stun()
    {
        if (!PlayerBatteryUIHandler.Instance)
            PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();

        var ray = new Ray(Flashlight.transform.position, Flashlight.transform.forward * Flashlight.Light.range);

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

        Flashlight.ConsumeBattery(Cost);
        flashSound = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.FlashlightStun);
        flashSound.start();

        timer = new CountdownTimer(1f);
        timer.Start();
        StartCoroutine(RestoreLightOverTime());
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
            Flashlight.Light.intensity = Mathf.Lerp(initialIntensity, 50 , delayTimer / buildUpTime);
            Flashlight.Light.color = Color.Lerp(flashlightColor, Color.red, delayTimer / buildUpTime);
            Flashlight.Light.spotAngle = Mathf.Lerp(lightSpotAngle, 5f, delayTimer / buildUpTime);
            Flashlight.Light.innerSpotAngle = Mathf.Lerp(initialInnerSpotAngle, 0f, delayTimer / buildUpTime);
            
            delayTimer += Time.deltaTime;
            yield return null;
        }

        //set light to stun flash properties 
        Flashlight.Light.intensity = lightIntensity;
        Flashlight.Light.color = flashColor;
        Flashlight.Light.spotAngle = 100;
        Flashlight.Light.innerSpotAngle = 50;

        Stun();
    }


    private IEnumerator RestoreLightOverTime()
    {
        Debug.Log("RestoreLightOverTime");
        while (timer != null && !timer.IsFinished)
        {
            // Gradually decrease intensity
            Flashlight.Light.intensity =
                Mathf.Lerp(Flashlight.Light.intensity, cooldownIntensity, decreaseRate * Time.deltaTime);

            // Gradually return color
            Flashlight.Light.color = Color.Lerp(Flashlight.Light.color, cooldownColor, decreaseRate * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }


        // Reset the flashlight when the timer finishes
        if (timer != null && timer.IsFinished)
        {
            Flashlight.ResetLight();
            timer = null;
        }
    }
}