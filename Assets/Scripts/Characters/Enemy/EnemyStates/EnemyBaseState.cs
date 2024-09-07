using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState : MonoBehaviour
{
    public EnemyClass enemy { get; set; }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate() { }
    public virtual void HandleAttack() { }
    public virtual void HandleDeath() { }
}
