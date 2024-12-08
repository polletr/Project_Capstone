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

        [field: SerializeField] public PickupData AbilityPickupData { get; private set; }
   
        protected FlashLight Flashlight;

        public abstract void OnUseAbility();
        public abstract void OnStopAbility();


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