using UnityEngine;

public class PlayerGetHitState : PlayerBaseState
{
    public PlayerGetHitState(PlayerController playerController) : base(playerController) { }

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
        AnimatorStateInfo stateInfo = playerAnimator.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == playerAnimator.GetHitHash) // Ensure this matches the animation state name
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                player.ChangeState(player.MoveState);

            }
        }

    }

}
