using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState
        (PlayerAnimator animator, PlayerController playerController, InputManager inputM)
        : base(animator, playerController, inputM) { }

    public override void EnterState()
    {
        playerAnimator.animator.Play(playerAnimator.DieHash);
        player.Event.OnPlayerDeath?.Invoke();
        //player.cam.transform.parent = null;
    }
    public override void ExitState()
    {
        //player.cam.transform.parent = null;
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {

    }

    public override void HandleMovement(Vector2 dir)
    {

    }


    public override void HandleGetHit()
    {

    }

    public override void HandleDeath()
    {

    }

    public override void HandleAttack(bool isHeld)
    {

    }

    public override void HandleFlashlightSphereCast()
    {

    }

    public override void HandleFlashlightPower()
    {

    }



}
