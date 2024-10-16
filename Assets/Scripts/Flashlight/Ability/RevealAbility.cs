using System;
using System.Collections;
using UnityEngine;

public class RevealAbility : FlashlightAbility
{
    [SerializeField] private float revealRange = 3f;
    private RevealableObject currentObj;
    private float originalIntesity;
    private Color originalColor;
    private Coroutine visualReveal;
    private Coroutine revealCoroutine;

    [SerializeField] private float noObjectinViewTime;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float timeToMaxIntensity;
    [SerializeField] Color targetColor;
    
    private MoveableObject moveableObject;

    private void Awake()
    {
        if(TryGetComponent( out moveableObject))
            moveableObject.enabled = false;
    }

    public override void OnStopAbility()
    {
        if (visualReveal != null)
            StopCoroutine(visualReveal);

        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
            if (currentObj != null)
            {
                if (!currentObj.isRevealed)
                {
                    currentObj.UnrevealObj();
                    currentObj = null;
                }
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
        bool check = false;
        while (!check)
        {
            if (Physics.Raycast(Flashlight.transform.position, Flashlight.transform.forward, out RaycastHit hit, revealRange))
            {

                if (hit.collider.TryGetComponent(out RevealableObject obj))
                {

                    if (currentObj == null)
                    {
                        currentObj = obj;
                    }
                    else if (currentObj != obj)
                    {
                        currentObj.UnrevealObj();
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
                        currentObj.UnrevealObj();
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
            Flashlight.Light.intensity = Mathf.Lerp(originalIntesity, maxIntensity, timer / timeToMaxIntensity);
            Flashlight.Light.color = Color.Lerp(originalColor, targetColor, timer / timeToMaxIntensity);
            yield return null;
        }
    }


}
