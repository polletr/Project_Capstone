using System.Collections;
using UnityEngine;

public class RevealAbility : FlashlightAbility
{
    [SerializeField] private float revealRange = 3f;

   // [SerializeField] private float noObjectinViewTime;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float timeToMaxIntensity;
    [SerializeField] private Color targetColor;

    private RevealableObject currentObj;

    private float originalIntensity;
    private Color originalColor;
    
    private Coroutine visualReveal;
    private Coroutine revealCoroutine;

    public override void Initialize(FlashLight flashlight)
    {
        base.Initialize(flashlight);
        originalIntensity = Flashlight.Light.intensity;
        originalColor = Flashlight.Light.color;
    }

    public override void OnUseAbility()
    {
        visualReveal = StartCoroutine(VisualReveal());
        revealCoroutine = StartCoroutine(UseRevealAbility());
    }

    public override void OnStopAbility()
    {
        if (visualReveal != null)
            StopCoroutine(visualReveal);

        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
            if (!currentObj && !currentObj.IsRevealed)
            {
                currentObj.UnRevealObj();
                currentObj = null;
            }
        }

        Flashlight.ConsumeBattery(Cost);

        Flashlight.ResetLight(Cooldown);
    }

    private IEnumerator UseRevealAbility()
    {
        var isRevealed = false;
        while (!isRevealed)
        {
            if (Physics.Raycast(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward, out var hit,
                    revealRange))
            {
                if (hit.collider.TryGetComponent(out RevealableObject obj))
                {
                    if (currentObj == null)
                    {
                        currentObj = obj;
                    }
                    else if (currentObj != obj)
                    {
                        currentObj.UnRevealObj();
                        currentObj = null;
                        OnStopAbility();
                        break;
                    }

                    currentObj.RevealObj(out isRevealed);
                }
                else
                {
                    if (currentObj != null)
                    {
                        currentObj.UnRevealObj();
                        currentObj = null;
                    }

                    OnStopAbility();
                    break;
                }
            }

            yield return null;
        }

        currentObj = null;
        OnStopAbility();
    }

    private IEnumerator VisualReveal()
    {
        var timer = 0f;
        while (Flashlight.Light.intensity < maxIntensity)
        {
            timer += Time.deltaTime;
            Flashlight.Light.intensity = Mathf.Lerp(originalIntensity, maxIntensity, timer / timeToMaxIntensity);
            Flashlight.Light.color = Color.Lerp(originalColor, targetColor, timer / timeToMaxIntensity);
            yield return null;
        }
    }
}