using System.Collections;
using FMOD.Studio;
using UnityEngine;

namespace Flashlight.Ability
{
    public class StunAbility : FlashlightAbility
    {
        [field: Header("Stun Ability Settings")] [SerializeField]
        private float stunRadius;

        private EventInstance flashSound;

        public override void OnUseAbility()
        {
            StartCoroutine(StartStunAttack());
        }


        public override void OnStopAbility()
        {
            StopAllCoroutines();
            Flashlight.ResetLight(0.5f);
        }

        private void Stun()
        {
            //set light to stun flash properties 
            Flashlight.Light.intensity = FinalIntensity;
            Flashlight.Light.color = FinalColor;
            Flashlight.Light.spotAngle = FinalSpotAngle;
            Flashlight.Light.innerSpotAngle = FinalInnerSpotAngle;

            if (!PlayerBatteryUIHandler.Instance)
                PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();

            var ray = new Ray(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward);
            var hits = Physics.SphereCastAll(ray, stunRadius, InteractRange);

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    var obj = hit.collider.gameObject;

                    if (!obj.TryGetComponent(out IStunable thing)) continue;
                
                    thing.ApplyStunEffect();
                }
            }

            flashSound = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.FlashlightStun);
            flashSound.start();
            Flashlight.ConsumeBattery(Cost);


            //set light to stun flash properties 
            Flashlight.StartCoroutine(Flashlight.ZeroOutLight(Cooldown));
            PlayerBatteryUIHandler.Instance.FlickerBatteryUIOnce();

            tutorialEvents.OnStun?.Invoke();
        }

        private IEnumerator StartStunAttack()
        {
            var timer = 0f;

            // Store the initial properties of the flashlight
            var initialIntensity = Flashlight.Light.intensity;
            var flashlightColor = Flashlight.Light.color;
            var lightSpotAngle = Flashlight.Light.spotAngle;
            var initialInnerSpotAngle = Flashlight.Light.innerSpotAngle;


            while (timer < AbilityBuildUpTime)
            {
                Flashlight.Light.intensity = Mathf.Lerp(initialIntensity, BuildupIntensity, timer / AbilityBuildUpTime);
                Flashlight.Light.color = Color.Lerp(flashlightColor, BuildupColor, timer / AbilityBuildUpTime);
                Flashlight.Light.spotAngle = Mathf.Lerp(lightSpotAngle, BuildupSpotAngle, timer / AbilityBuildUpTime);
                Flashlight.Light.innerSpotAngle = Mathf.Lerp(initialInnerSpotAngle, BuildupInnerSpotAngle, timer / AbilityBuildUpTime);

                timer += Time.deltaTime;
                yield return null;
            }

            Stun();
        }
    }
}