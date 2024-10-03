using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    public PlayerInteractState(PlayerAnimator animator, PlayerController playerController, InputManager inputM) 
        : base(animator, playerController, inputM) {}

    public override void EnterState()
    {
        player.interactableObj.OnInteract();
        player.ChangeState(player.MoveState);
    }
    public override void ExitState()
    {
        player.interactableObj = null;
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        StepsSound();
    }

    public override void HandleMovement(Vector2 dir)
    {

    }

}
