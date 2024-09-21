using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealableObject : MonoBehaviour  , IRevealable
{
    public void ApplyEffect()
    {
        Debug.Log("Revealable object");
    }
}
