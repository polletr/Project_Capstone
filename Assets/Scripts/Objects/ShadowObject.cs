using UnityEngine;
using UnityEngine.Events;

public class ShadowObject : MonoBehaviour, IStunable
{
    public bool CanApplyEffect { get; set; }

    [SerializeField] UnityEvent StunObject;

    Outline outline;
    [SerializeField] bool applyOutline = true;


    private void Start()
    {
        CanApplyEffect = true;
        outline = GetComponent<Outline>();

    }

    public void ApplyStunEffect()
    {
        Debug.Log("Enemy Received Stun");
        StunObject.Invoke();
    }

    public void ApplyEffect()
    {
        if (applyOutline)
            outline.AppyOutlineEffect();

    }

    public void RemoveEffect()
    {
        if (applyOutline)
            outline.RemoveOutlineEffect();
    }

}
