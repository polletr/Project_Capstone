using System.Collections;
using UnityEngine;

public class DisapearAbility : FlashlightAbility
{
   
    [SerializeField] private float interactRange = 3f;
    
    private DisapearObject currentObj;
    
    private Coroutine visualReveal;
    private Coroutine revealCoroutine;

    public override void OnUseAbility()
    {
        visualReveal = StartCoroutine(ChangeRevealLight());
        revealCoroutine = StartCoroutine(UseDisappearAbility());
    }

    public override void OnStopAbility()
    {
        if (visualReveal != null)
            StopCoroutine(visualReveal);

        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
            if (currentObj && !currentObj.IsHidden)
            {
                currentObj.UnRevealObj();
                currentObj = null;
            }
        }


        Flashlight.ResetLight(1);
    }

    private IEnumerator UseDisappearAbility()
    {
        var isRevealed = false;
        while (!isRevealed)
        {
          
            if (Physics.Raycast(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward, out var hit, interactRange))
            {
                if (hit.collider.TryGetComponent(out DisapearObject obj))
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

                    currentObj.HideObj(out isRevealed);
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
        Flashlight.ConsumeBattery(Cost);
        Flashlight.ResetLight(Cooldown);
    }

    private IEnumerator ChangeRevealLight()
    {
        var timer = 0f;
        
        var startIntensity = Flashlight.Light.intensity;
        var startColor = Flashlight.Light.color;
       
        
        while (Flashlight.Light.intensity < AbilityBuildUpTime)
        {
            Flashlight.Light.intensity = Mathf.Lerp(startIntensity, AbilityIntensity, timer / AbilityBuildUpTime);
            Flashlight.Light.color = Color.Lerp(startColor, AbilityColor, timer / AbilityBuildUpTime);
            
            timer += Time.deltaTime;
            yield return null;
        }
    }
}

