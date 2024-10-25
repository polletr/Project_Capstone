using UnityEngine;
using Utilities;
public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerController playerController)
        : base(playerController) { }
    
    public override void EnterState()
    {
        //Use current Flashlight ability Attack
        Player.flashlight.HandleFlashAbility();
    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {

    }
    
    /*public override void HandleAttack(bool isHeld)
    {
        if(!isHeld)
        {
            Player.flashlight.StopUsingFlashlight();
        } 
    }*/

    protected override void HandleFlashlightSphereCast()
    {

    }
    
    public override void HandleFlashlightPower()
    {

    }

    
    


}
