using System.Collections;
using UnityEngine;

namespace Flashlight.Ability
{
    public class RevealAbility : FlashlightAbility
    {
        private RevealableObject currentObj;

        private Coroutine visualReveal;
        private Coroutine revealCoroutine;

        public override void OnUseAbility()
        {
            visualReveal = StartCoroutine(ChangeRevealLight());
            revealCoroutine = StartCoroutine(UseRevealAbility());
        }

        public override void OnStopAbility()
        {
            if (visualReveal != null)
                StopCoroutine(visualReveal);

            if (revealCoroutine != null)
            {
                StopCoroutine(revealCoroutine);
                if (currentObj && !currentObj.IsRevealed)
                {
                    currentObj.UnRevealObj();
                    currentObj = null;
                }
            }

            Flashlight.ResetLight(0.5f);
        }

        private IEnumerator UseRevealAbility()
        {
            var isRevealed = false;
            while (!isRevealed)
            {
                if (Physics.Raycast(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward, out var hit,
                        Flashlight.InteractRange))
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

            Flashlight.Light.intensity = Flashlight.FinalIntensity;
            Flashlight.Light.color = Flashlight.FinalColor;
            Flashlight.Light.spotAngle = Flashlight.FinalSpotAngle;
            Flashlight.Light.innerSpotAngle = Flashlight.FinalInnerSpotAngle;

            currentObj = null;
            Flashlight.ConsumeBattery(Cost);
            Flashlight.StartCoroutine(Flashlight.ZeroOutLight(Cooldown));

            if (!PlayerBatteryUIHandler.Instance)
                PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();

            tutorialEvents.OnReveal?.Invoke();
            StopAllCoroutines();
        }

        private IEnumerator ChangeRevealLight()
        {
            var timer = 0f;

            var startIntensity = Flashlight.Light.intensity;
            var startColor = Flashlight.Light.color;


            while (timer < AbilityBuildUpTime)
            {
                timer += Time.deltaTime;
                Flashlight.Light.intensity = Mathf.Lerp(startIntensity, Flashlight.BuildupIntensity, timer / AbilityBuildUpTime);
                Flashlight.Light.color = Color.Lerp(startColor, Flashlight.BuildupColor, timer / AbilityBuildUpTime);

                yield return null;
            }
        }
    }
}