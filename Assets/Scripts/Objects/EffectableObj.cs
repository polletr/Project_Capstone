using UnityEngine;
using UnityEngine.Events;

public class EffectableObj : MonoBehaviour, IEffectable
{
    public bool CanApplyEffect { get; set; }

    public UnityEvent OnApplyEffect;
    public UnityEvent OnRemoveEffect;


    public virtual void ApplyEffect() 
    {
        Debug.Log("Apply");
        OnApplyEffect.Invoke();
    }

    public virtual void RemoveEffect()
    {
        Debug.Log("remove");

        OnRemoveEffect.Invoke();
    }

}
