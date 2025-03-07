using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

namespace Flashlight.Ability
{
    public class DisappearAbility : FlashlightAbility
    {
        private DisappearObject currentObj;

        private Coroutine visualReveal;

        public override void OnUseAbility()
        {
            visualReveal = StartCoroutine(ChangeRevealLight());
            UseDisappearAbility();
        }

        public override void OnStopAbility()
        {
            if (visualReveal != null)
                StopCoroutine(visualReveal);

            if (currentObj && !currentObj.IsHidden)
            {
                currentObj.UnHideObj();
                currentObj = null;
            }
            Flashlight.CurrentAbility = null;
            Flashlight.ResetLight(0.5f);
        }

        private void UseDisappearAbility()
        {
            var isRevealed = false;
            if (Physics.Raycast(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward, out var hit,
                    Flashlight.InteractRange))
            {
                if (hit.collider.TryGetComponent(out DisappearObject obj))
                {
                    currentObj = obj;
                    currentObj.HideObj(out isRevealed);

                }
                else
                {
                    OnStopAbility();
                }
            }

            Flashlight.NewIntensity = Flashlight.FinalIntensity;
            Flashlight.Light.color = Flashlight.FinalColor;
            Flashlight.Light.spotAngle = Flashlight.FinalSpotAngle;
            Flashlight.Light.innerSpotAngle = Flashlight.FinalInnerSpotAngle;

            Flashlight.ConsumeBattery(Cost);
            Flashlight.StartCoroutine(Flashlight.ZeroOutLight(Cooldown));

            if (PlayerBatteryUIHandler.Instance == null)
                PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();

            tutorialEvents.OnDisappear?.Invoke();
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
                Flashlight.NewIntensity = Mathf.Lerp(startIntensity,    Flashlight.BuildupIntensity, timer / AbilityBuildUpTime);
                Flashlight.Light.color = Color.Lerp(startColor,    Flashlight.BuildupColor, timer / AbilityBuildUpTime);

                yield return null;
            }
        }
    }
}