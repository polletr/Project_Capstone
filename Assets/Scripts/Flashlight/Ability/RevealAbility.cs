using System.Collections;
using UnityEngine;

public class RevealAbility : FlashlightAbility
{
    [SerializeField] private float revealRange = 3f;
    private RevealableObject currentObj;
    private float originalIntensity;
    private Color originalColor;
    private Coroutine visualReveal;
    private Coroutine revealCoroutine;

    [SerializeField] private float noObjectinViewTime;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float timeToMaxIntensity;
    [SerializeField] private Color targetColor;
    
    public override void OnStopAbility()
    {
        if (visualReveal != null)
            StopCoroutine(visualReveal);

        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
            if (currentObj != null && !currentObj.IsRevealed)
            {
                currentObj.UnRevealObj();
                currentObj = null;
            }
        }

        Flashlight.ConsumeBattery(Cost);

        Flashlight.ResetLight();
    }

    public override void OnUseAbility()
    {
        visualReveal = StartCoroutine(VisualReveal());
        revealCoroutine = StartCoroutine(UseRevealAbility());
    }

    private IEnumerator UseRevealAbility()
    {
        var check = false;
        while (!check)
        {
            if (Physics.Raycast(Flashlight.transform.position, Flashlight.transform.forward, out RaycastHit hit,
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

                    currentObj.RevealObj(out check);
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
        float timer = 0f;
        while (Flashlight.Light.intensity < maxIntensity)
        {
            timer += Time.deltaTime;
            Flashlight.Light.intensity = Mathf.Lerp(originalIntensity, maxIntensity, timer / timeToMaxIntensity);
            Flashlight.Light.color = Color.Lerp(originalColor, targetColor, timer / timeToMaxIntensity);
            yield return null;
        }
    }
}