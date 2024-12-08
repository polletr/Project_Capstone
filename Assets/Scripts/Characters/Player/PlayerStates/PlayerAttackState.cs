public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerController playerController)
        : base(playerController) { }
    
    public override void EnterState()
    {
        //Use current Flashlight ability Attack
        Player.flashlight.HandleAbility();
       // Player.ChangeState(Player.MoveState);
    }
    public override void ExitState()
    {
        HandleFlashlightSphereCast();
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
