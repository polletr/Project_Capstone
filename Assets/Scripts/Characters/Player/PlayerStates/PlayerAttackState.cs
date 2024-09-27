using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState (PlayerAnimator animator, PlayerController playerController, InputManager inputM) : base(animator, playerController, inputM)
    {
 
    }
    public override void EnterState()
    {
        //Use current Flashlight ability Attack
        if (player.HasFlashlight)
        {
            player.flashlight.HandleFlashAblility();
        }
        else
        {
            player.ChangeState(player.MoveState);
        }
    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {

    }
    
    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void HandleAttack(bool isHeld)
    {
      if(!isHeld && player.HasFlashlight)
      {
          player.flashlight.StopUsingFlashlight();
      } 
    }

    public override void HandleFlashlightSphereCast()
    {

    }

    public override void HandleMove()
    {
        player.ChangeState(player.MoveState);
    }

    public override void HandleFlashlightPower()
    {

    }


}
