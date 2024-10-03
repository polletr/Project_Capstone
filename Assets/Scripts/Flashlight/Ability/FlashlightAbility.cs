using UnityEngine;

public abstract class FlashlightAbility : MonoBehaviour
{
    [SerializeField] protected int cost;
    [SerializeField] protected float cooldown;

    protected FlashLight _flashlight;

    public abstract void OnUseAbility();
    public abstract void OnStopAbility();


    public virtual void Initialize(FlashLight flashlight)
    {
        _flashlight = flashlight;
    }
}
