using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : PlayerBaseState
{
    public override void EnterState()
    {
        isRunning = false;

        //Check if weapon is one-handed or two
        player.animator.Play(OneHandAimHash);

        //player.animator.Play(TwoHandAimHash);
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

    public override void HandleAttack()
    {
        //Do Shoot Logic!
    }

    public override void HandleRun(bool check)
    {

    }

    public override void HandleAim(bool check)
    {
        if (!check)
            player.ChangeState(new PlayerMoveState());
    }


    protected override float GetSpeed()
    {
        return player.Settings.AimingSpeed;
    }

    protected override float GetSoundEmitted()
    {
        return player.Settings.CrouchSoundRange;
    }

    protected override float GetMovementAnimValue()
    {
        return 0.3f;
    }


}
