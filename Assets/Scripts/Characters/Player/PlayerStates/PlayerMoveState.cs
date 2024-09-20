using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public override void EnterState()
    {
        player.animator.Play(IdleHash);
    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate() { }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void HandleChangeAbility(int d)
    {
        player.flashlight.ChangeSelectedAbility(d);
    }

}
