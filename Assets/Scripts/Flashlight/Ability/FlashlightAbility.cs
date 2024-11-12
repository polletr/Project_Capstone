using UnityEngine;

namespace Flashlight.Ability
{
    public abstract class FlashlightAbility : MonoBehaviour
    {
        public TutorialEvents tutorialEvents;
    
        // Ability settings 
        [field: Header("Ability Settings")]
        [field: SerializeField] public int Cost { get; protected set; } = 10;
        [field: SerializeField] public float Cooldown { get; protected set; } = 10;
        [field: SerializeField] public float AbilityBuildUpTime { get; protected set; }
        [field: SerializeField] public float InteractRange { get; protected set; }
        [field: SerializeField] public float CloseRangeInnerAngle { get; protected set; }
        [field: SerializeField] public float CloseRangeIntensity { get; protected set; }
    

        // Ability flashlight settings
        [field: Header("Base Flashlight Settings")]
        [field: SerializeField] public float BaseIntensity { get; protected set; } = 25;
        [field: SerializeField] public Color BaseColor { get; protected set; }
        [field: SerializeField] public float BaseSpotAngle { get; protected set; } = 60;
        [field: SerializeField] public float BaseInnerSpotAngle { get; protected set; } = 30;
        [field: SerializeField] public float BaseRange { get; protected set; } = 10;
        
        // Ability light settings
        [field: Header("Build Up Flashlight Settings")]
        [field: SerializeField] public float BuildupIntensity { get; protected set; }
        [field: SerializeField] public Color BuildupColor { get; protected set; }
        [field: SerializeField] public float BuildupSpotAngle { get; protected set; }
        [field: SerializeField] public float BuildupInnerSpotAngle { get; protected set; }

        [field: Header("Finish Flashlight Settings")]
        [field: SerializeField] public float FinalIntensity { get; protected set; }
        [field: SerializeField] public Color FinalColor { get; protected set; }
        [field: SerializeField] public float FinalSpotAngle { get; protected set; }
        [field: SerializeField] public float FinalInnerSpotAngle { get; protected set; }


        [field: SerializeField] public PickupData AbilityPickupData { get; private set; }
   
        protected FlashLight Flashlight;

        public abstract void OnUseAbility();
        public abstract void OnStopAbility();

        public void SetLight(Light currentLight)
        {
            currentLight.intensity = BaseIntensity;
            currentLight.color = BaseColor;
            currentLight.spotAngle = BaseSpotAngle;
            currentLight.innerSpotAngle = BaseInnerSpotAngle;
            currentLight.range = BaseRange;
        }

        public virtual void Initialize(FlashLight flashlight)
        {
            Flashlight = flashlight;
        }
    
        public void SetPickupData(PickupData data)
        {
            AbilityPickupData = data;
        }
    }
}