using UnityEngine;
using Utilities;
public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerController playerController)
        : base(playerController) { }
    
    private StopwatchTimer timer;
    bool usingAbility = false;

    private float clickTimeFrame = 0.2f;
    public override void EnterState()
    {
        timer = new StopwatchTimer();
        timer.Start();
        usingAbility = false;
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
        
        if (timer.GetTime() > clickTimeFrame && !usingAbility)
        {
            usingAbility = true;
           
            player.flashlight.HandleFlashAbility();
        }
    }

    public override void HandleAttack(bool isHeld)
    {
        if (isHeld || !player.HasFlashlight) return;
        
        if (timer.GetTime() < clickTimeFrame)
        {
            player.flashlight.HandleStunAbility();
        }
        player.ChangeState(player.MoveState);
    
        player.flashlight.StopUsingFlashlight();
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
