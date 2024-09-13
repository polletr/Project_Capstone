using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField, Range(0.1f, 2f)]
    private float speed = 1f;

    public float Speed { get { return speed; } }





}
