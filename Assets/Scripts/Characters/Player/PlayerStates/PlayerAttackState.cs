using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public override void EnterState()
    {
        player.animator.Play(AttackHash);

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
        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == AttackHash) // Ensure this matches the animation state name
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                player.ChangeState(new PlayerMoveState());
            }
        }
    }

    public override void HandleMovement(Vector2 dir)
    {
        _direction = new Vector3(dir.x, 0, dir.y);
    }


}
