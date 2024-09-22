using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackState : PlayerBaseState
{
    public override void EnterState()
    {
        //Use current Flashlight ability Attack
        player.flashlight.HandleFlashAblility();
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
      if(!isHeld)
      {
            player.flashlight.StopUsingFlashlight();
      } 
    }

    public override void HandleFlashlightSphereCast()
    {

    }

    public override void HandleMove()
    {
        player.ChangeState(new PlayerMoveState());
    }

    public override void HandleFlashlightPower()
    {

    }


}
