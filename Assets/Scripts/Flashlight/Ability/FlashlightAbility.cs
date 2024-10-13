using UnityEngine;

public abstract class FlashlightAbility : MonoBehaviour
{
    [field: SerializeField] public int Cost { get; private set; } = 10;
    [SerializeField] protected float cooldown;

    protected FlashLight _flashlight;

    public abstract void OnUseAbility();
    public abstract void OnStopAbility();


    public virtual void Initialize(FlashLight flashlight)
    {
        _flashlight = flashlight;
    }
}
