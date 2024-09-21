using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class MoveableObject : MonoBehaviour , IMovable
{
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void ApplyEffect()
    {
       Debug.Log("Moveable object");
    }
}
