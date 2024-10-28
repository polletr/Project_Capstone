using UnityEngine;

public abstract class FlashlightAbility : MonoBehaviour
{
    [field: SerializeField] public int Cost { get; protected set; } = 10;
    
    [field: SerializeField] public float Cooldown { get; protected set; } = 10;

    protected FlashLight Flashlight;

    public abstract void OnUseAbility();
    public abstract void OnStopAbility();


    public virtual void Initialize(FlashLight flashlight)
    {
        Flashlight = flashlight;
    }
}
