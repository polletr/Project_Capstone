using UnityEngine;
using Utilities;
public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerController playerController)
        : base(playerController) { }
    
    private StopwatchTimer timer;
    bool usingAbility = false;
    public override void EnterState()
    {
        timer = new StopwatchTimer();
        timer.Start();
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
        timer.Tick(Time.deltaTime);
        
        if (timer.GetTime() > 0.1f && !usingAbility)
        {
            usingAbility = true;
           
            player.flashlight.HandleFlashAbility();
        }
    }

    public override void HandleAttack(bool isHeld)
    {
        if (!isHeld)
        {
            if (timer.GetTime() < 0.1f)
            {
               // stun!
               player.flashlight.HandleStunAbility();
            }
            player.ChangeState(player.MoveState);
        }
        if (!isHeld && player.HasFlashlight)
        {
            player.flashlight.StopUsingFlashlight();
        }
    }

    protected override void HandleFlashlightSphereCast()
    {

    }

    public override void HandleMove()
    {
        //player.ChangeState(player.MoveState);
    }

    public override void HandleFlashlightPower()
    {

    }
    
    


}
