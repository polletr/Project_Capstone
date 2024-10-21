using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Outline))]
public class MoveableObject : MonoBehaviour, IMovable
{
    //[SerializeField] private float breakForce = 4f;
    [SerializeField] private Material outlineMaterial;
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
       // GetComponent<Renderer>().material += outlineMaterial;
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

    #region CollisonDrop

    /*  private void OnCollisionEnter(Collision collision)
      {
          if (IsPicked && collision.relativeVelocity.magnitude > breakForce)
              IsPicked = false;
      }

      public IEnumerator Pickup()
      {
          yield return new WaitForSecondsRealtime(5f);

          IsPicked = true;
      }*/

    #endregion
}