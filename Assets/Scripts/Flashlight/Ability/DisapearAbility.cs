using System.Collections;
using UnityEngine;

public class DisapearAbility : FlashlightAbility
{
   
    
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
                currentObj.UnHideObj();
                currentObj = null;
            }
        }


        Flashlight.ResetLight(0.5f);
    }

    private IEnumerator UseDisappearAbility()
    {
        var isRevealed = false;
        while (!isRevealed)
        {
          
            if (Physics.Raycast(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward, out var hit, InteractRange))
            {
                if (hit.collider.TryGetComponent(out DisapearObject obj))
                {
                    if (currentObj == null)
                    {
                        currentObj = obj;
                    }
                    else if (currentObj != obj)
                    {
                        currentObj.UnHideObj();
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
                        currentObj.UnHideObj();
                        currentObj = null;
                    }

                    OnStopAbility();
                    break;
                }
            }

            yield return null;
        }

        Flashlight.Light.intensity = AbilityIntensity;
        Flashlight.Light.color = AbilityColor;
        Flashlight.Light.spotAngle = AbilitySpotAngle;
        Flashlight.Light.innerSpotAngle = AbilityInnerSpotAngle;
        
        currentObj = null;
        Flashlight.ConsumeBattery(Cost);
        Flashlight.ResetLight(Cooldown);
        if (!PlayerBatteryUIHandler.Instance)
            PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();
        
        tutorialEvents.OnDisappear?.Invoke();
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

