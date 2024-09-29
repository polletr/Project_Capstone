using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerAnimator animator, PlayerController playerController, InputManager inputM) : base(animator, playerController, inputM)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Player Dead");
    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        StepsSound();
    }

    public override void HandleMovement(Vector2 dir)
    {

    }


    public override void HandleGetHit()
    {

    }

    public override void HandleDeath()
    {

    }

    public override void HandleAttack(bool isHeld)
    {

    }

    public override void HandleFlashlightSphereCast()
    {

    }

    public override void HandleFlashlightPower()
    {

    }



}
