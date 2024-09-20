using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlashlightAbility : MonoBehaviour
{
    public abstract int Cost { get; set; }
    public abstract float Cooldown { get; set; }
   
    public abstract void OnUseAbility();
    public abstract void OnStopAbility();

}
