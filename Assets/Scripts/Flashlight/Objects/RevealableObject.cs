using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealableObject : MonoBehaviour  , IRevealable
{
    [SerializeField] private Material OnMaterial;
    [SerializeField] private Material OffMaterial;
    public void ApplyEffect()
    {
        GetComponent<MeshRenderer>().material = OnMaterial;
    }

    public void RemoveEffect()
    {
        GetComponent<MeshRenderer>().material = OffMaterial;
    }
}
