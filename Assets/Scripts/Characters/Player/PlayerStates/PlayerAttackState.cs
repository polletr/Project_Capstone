using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerController playerController)
        : base(playerController) { }
    
    public override void EnterState()
    {
        //Use current Flashlight ability Attack
        Player.flashlight.HandleAbility();
        Debug.Log("Enter Attack State");
       // Player.ChangeState(Player.MoveState);
    }
    public override void ExitState()
    {
        HandleFlashlightSphereCast();
        Debug.Log("Exit Attack State");

    }

    public override void StateFixedUpdate()
    {

    }
    
    public override void HandleAttack(bool isHeld)
    {
        if(!isHeld)
        {
            Player.flashlight.StopUsingFlashlight();
            Player.ChangeState(Player.MoveState);
        } 
    }

    
    public override void HandleFlashlightPower()
    {

    }
}
