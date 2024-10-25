using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class StunAbility : FlashlightAbility
{
    [SerializeField] private Color flashColor;
    [SerializeField] private Color cooldownColor;
    [SerializeField] private float decreaseRate;
    [SerializeField] private float effectRadius;
    [SerializeField] private float lightIntensity;
    [SerializeField] private float cooldownIntensity;

    private CountdownTimer timer;
    private EventInstance flashSound;

    public override void OnUseAbility()
    {
        if (!PlayerBatteryUIHandler.Instance)
            PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();
        
        timer = new CountdownTimer(cooldown);
        timer.Start();

        Flashlight.Light.intensity = lightIntensity;
        Flashlight.Light.color = flashColor;

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

        StartCoroutine(RestoreLightOverTime());

    }

    public override void OnStopAbility()
    {

    }

    private IEnumerator RestoreLightOverTime()
    {
        while (timer != null && !timer.IsFinished)
        {
            timer.Tick(Time.deltaTime);
            // Gradually decrease intensity
            Flashlight.Light.intensity = Mathf.Lerp(Flashlight.Light.intensity, cooldownIntensity, decreaseRate * Time.deltaTime);

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
