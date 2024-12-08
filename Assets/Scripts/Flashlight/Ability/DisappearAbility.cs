using System.Collections;
using UnityEngine;

namespace Flashlight.Ability
{
    public class DisappearAbility : FlashlightAbility
    {
        private DisappearObject currentObj;

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
                if (Physics.Raycast(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward, out var hit,
                        Flashlight.InteractRange))
                {
                    if (hit.collider.TryGetComponent(out DisappearObject obj))
                    {
                        if (!currentObj)
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

            Flashlight.Light.intensity = Flashlight.FinalIntensity;
            Flashlight.Light.color = Flashlight.FinalColor;
            Flashlight.Light.spotAngle = Flashlight.FinalSpotAngle;
            Flashlight.Light.innerSpotAngle = Flashlight.FinalInnerSpotAngle;

            currentObj = null;
            Flashlight.ConsumeBattery(Cost);


            if (PlayerBatteryUIHandler.Instance == null)
                PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();

            tutorialEvents.OnDisappear?.Invoke();
            Flashlight.StartCoroutine(Flashlight.ZeroOutLight(Cooldown));
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
                Flashlight.Light.intensity = Mathf.Lerp(startIntensity,    Flashlight.BuildupIntensity, timer / AbilityBuildUpTime);
                Flashlight.Light.color = Color.Lerp(startColor,    Flashlight.BuildupColor, timer / AbilityBuildUpTime);

                yield return null;
            }
        }
    }
}