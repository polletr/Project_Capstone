using UnityEngine;

public abstract class FlashlightAbility : MonoBehaviour
{
    // Ability settings 
    [field: SerializeField] public int Cost { get; protected set; } = 10;
    [field: SerializeField] public float Cooldown { get; protected set; } = 10;

    // Ability light settings
    [field: SerializeField] public float AbilityIntensity { get; protected set; }
    [field: SerializeField] public Color AbilityColor { get; protected set; }
    [field: SerializeField] public float AbilitySpotAngle { get; protected set; }
    [field: SerializeField] public float AbilityInnerSpotAngle { get; protected set; }
    [field: SerializeField] public float AbilityBuildUpTime { get; protected set; }

    protected FlashLight Flashlight;

    public abstract void OnUseAbility();
    public abstract void OnStopAbility();


    public virtual void Initialize(FlashLight flashlight)
    {
        Flashlight = flashlight;
    }
}