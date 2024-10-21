using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Outline))]
public class MoveableObject : MonoBehaviour, IMovable
{
   public bool DefaultUseGravity { get; private set; }
    public float DefaultDrag { get; private set; }
    public RigidbodyConstraints DefaultConstraints { get; private set; }
    
    public Outline BoxOutline {get; private set;}
    public Rigidbody Rb { get; set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        BoxOutline = GetComponent<Outline>();
        BoxOutline.enabled = false;
        DefaultUseGravity = Rb.useGravity;
        DefaultDrag = Rb.drag;
        DefaultConstraints = Rb.constraints;
    }

    public void ApplyEffect()
    {
        BoxOutline.enabled = true;
    }
    
    public void RemoveEffect()
    {
        BoxOutline.enabled = false;
    }

}