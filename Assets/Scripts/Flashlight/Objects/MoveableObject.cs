using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MoveableObject : MonoBehaviour, IMovable
{

    //[SerializeField] private float breakForce = 4f;
    [field: SerializeField] public bool IsPicked { get; set; }

    public bool DefaultUseGravity { get; private set; }
    public float DefaultDrag { get; private set; }
    public RigidbodyConstraints DefaultConstraints { get; private set; }

    public Rigidbody Rb { get; set; }

    private void Awake()
    {
        IsPicked = false;
        Rb = GetComponent<Rigidbody>();

        DefaultUseGravity = Rb.useGravity;
        DefaultDrag = Rb.drag;
        DefaultConstraints = Rb.constraints;
    }

    public void ApplyEffect()
    {
        //outline effect
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
