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

    public override void OnUseAbility()
    {
        timer = new CountdownTimer(cooldown);
        timer.Start();

        _flashlight.Light.intensity = lightIntensity;
        _flashlight.Light.color = flashColor;

        Ray ray = new Ray(transform.position, transform.forward * _flashlight.Light.range);
        RaycastHit[] hits = Physics.SphereCastAll(ray, effectRadius, _flashlight.Light.range);
        foreach (RaycastHit hit in hits)
        {
            var obj = hit.collider.gameObject;
            Debug.Log("Flash Hit" + obj);

            if (obj.TryGetComponent(out IStunnable thing))
                thing.ApplyEffect();
        }

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
            _flashlight.Light.intensity = Mathf.Lerp(_flashlight.Light.intensity, cooldownIntensity, decreaseRate * Time.deltaTime);

            // Gradually return color
            _flashlight.Light.color = Color.Lerp(_flashlight.Light.color, cooldownColor, decreaseRate * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }

        // Reset the flashlight when the timer finishes
        if (timer != null && timer.IsFinished)
        {
            _flashlight.ResetLight(cost);
            timer = null;
        }
    }
}
