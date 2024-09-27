using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetHitState : PlayerBaseState
{
    public PlayerGetHitState(PlayerAnimator animator, PlayerController playerController, InputManager inputM) : base(animator, playerController, inputM)
    {
    }

    public override void EnterState()
    {
        playerAnimator.animator.Play(playerAnimator.GetHitHash);
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
        AnimatorStateInfo stateInfo =  playerAnimator.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == playerAnimator.GetHitHash) // Ensure this matches the animation state name
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                player.ChangeState(player.MoveState);

            }
        }

    }


    public override void HandleMovement(Vector2 dir)
    {
        _direction = new Vector3(dir.x, 0, dir.y);
    }

}
