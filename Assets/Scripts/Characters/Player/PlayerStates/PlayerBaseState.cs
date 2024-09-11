using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : MonoBehaviour
{

    protected static readonly int IdleHash = Animator.StringToHash("Idle");
    protected static readonly int RunHash = Animator.StringToHash("Run");
    protected static readonly int WalkHash = Animator.StringToHash("Walk");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int GetHitHash = Animator.StringToHash("GetHit");
    protected static readonly int DieHash = Animator.StringToHash("Die");

    protected Vector3 _direction;

    public PlayerController player { get; set; }
    public InputManager inputManager { get; set; }
    public virtual void EnterState()
    {

    }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate()
    {

    }
    public virtual void StateUpdate() { }
    public virtual void HandleMovement(Vector2 dir) { }
    public virtual void HandleAttack()
    {
        player.ChangeState(new PlayerAttackState());
    }

    public virtual void HandleInteract()
    {
        player.ChangeState(new PlayerInteractState());

    }

    public virtual void HandleGetHit()
    {
        player.ChangeState(new PlayerGetHitState());

    }


    public virtual void StopInteract() { }

    public virtual void HandleDeath() 
    {
        player.ChangeState(new PlayerDeathState());
    }

    protected void Rotate()
    {
        player.gameObject.transform.rotation = Quaternion.Slerp(player.gameObject.transform.rotation, player.PlayerRotation, player.Settings.RotationSpeed);
    }


}

