using UnityEngine;

public abstract class FlashlightAbility : MonoBehaviour
{
    public TutorialEvents tutorialEvents;
    
    // Ability settings 
    [field: SerializeField] public int Cost { get; protected set; } = 10;
    [field: SerializeField] public float Cooldown { get; protected set; } = 10;
    [field: SerializeField] public float AbilityBuildUpTime { get; protected set; }

    // Ability light settings
    [field: Header("Ability Light Settings")]
    [field: SerializeField]
    public float AbilityIntensity { get; protected set; }

    [field: SerializeField] public Color AbilityColor { get; protected set; }
    [field: SerializeField] public float AbilitySpotAngle { get; protected set; }
    [field: SerializeField] public float AbilityInnerSpotAngle { get; protected set; }

    // Ability flashlight settings
    [field: Header("Flashlight Settings")]
    [field: SerializeField]
    public float BaseIntensity { get; protected set; } = 25;

    [field: SerializeField] public Color BaseColor { get; protected set; }
    [field: SerializeField] public float BaseSpotAngle { get; protected set; } = 60;
    [field: SerializeField] public float BaseInnerSpotAngle { get; protected set; } = 30;
    [field: SerializeField] public float BaseRange { get; protected set; } = 10;

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