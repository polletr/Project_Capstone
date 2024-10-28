
using System;
using UnityEngine;
using UnityEngine.Events;

public class GenericCollider : MonoBehaviour
{ 
    public event Action<Collider, Vector3> TriggerEnter, TriggerStay, TriggerExit;
    public UnityEvent<Collider, Vector3> UTriggerEnter, UTriggerStay, UTriggerExit;
    public event Action<Collision> CollisionEnter, CollisionStay, CollisionExit;
    public UnityEvent<Collision> UCollisionEnter, UCollisionStay, UCollisioNExit;
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter?.Invoke(other, transform.forward);
        UTriggerEnter?.Invoke(other, transform.forward);
        
    }

    private void OnTriggerStay(Collider other)
    {
        
        TriggerStay?.Invoke(other, transform.forward);
        UTriggerStay.Invoke(other, transform.forward);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit?.Invoke(other, transform.forward);
        UTriggerExit?.Invoke(other, transform.forward);
    }
}
