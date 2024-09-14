using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : Weapon
{
    [SerializeField]
    private bool automatic;

    [SerializeField]
    protected float cooldown;

    protected float currentCooldown;

    private float ammoCount;

    void Start()
    {
        currentCooldown = cooldown;
    }

}
