using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class StunAbility : FlashlightAbility
{
    [SerializeField] float effectRadius;
    [SerializeField] Color flashColor;
    [SerializeField] Color cooldownColor;
    [SerializeField] float decreaseRate;


    [SerializeField] float lightIntensity;

    [SerializeField] float cooldownIntensity;

    CountdownTimer timer;

    EventInstance flashSound;

    public override void OnUseAbility()
    {
        timer = new CountdownTimer(cooldown);
        timer.Start();

        Flashlight.Light.intensity = lightIntensity;
        Flashlight.Light.color = flashColor;

        Ray ray = new Ray(transform.position, transform.forward * Flashlight.Light.range);
        RaycastHit[] hits = Physics.SphereCastAll(ray, effectRadius, Flashlight.Light.range);
        foreach (RaycastHit hit in hits)
        {
            var obj = hit.collider.gameObject;

            if (obj.TryGetComponent(out IStunnable thing))
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
