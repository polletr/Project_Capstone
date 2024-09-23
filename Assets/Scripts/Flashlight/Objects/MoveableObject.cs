using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class MoveableObject : MonoBehaviour, IMovable
{

    [SerializeField] private float breakForce = 4f;

    [field: SerializeField] public bool IsPicked { get; set; }

    public Rigidbody Rb { get; set; }

    private void Awake()
    {
        IsPicked = false;
        Rb = GetComponent<Rigidbody>();
    }

    public void ApplyEffect()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsPicked && collision.relativeVelocity.magnitude > breakForce)
            IsPicked = false;
    }

    public IEnumerator Pickup()
    {
        yield return new WaitForSecondsRealtime(5f);

        IsPicked = true;
    }
}
