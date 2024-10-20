using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractState : PlayerBaseState
{
    public PlayerInteractState(PlayerController playerController) : base(playerController)
    {
    }

    public override void EnterState()
    {
        player.interactableObj.OnInteract();

        if (player.interactableObj is not Documentation)
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
    }

    public override void HandleMovement(Vector2 dir)
    {
    }

    public override void HandleLookAround(Vector2 dir, InputDevice device)
    {
    }

    public override void HandleRecharge()
    {
    }

    public override void HandleAttack(bool isHeld)
    {
        
    }

    public override void HandleInteract()
    {
        if (player.interactableObj is not Documentation) return;

        player.interactableObj.OnInteract();
        player.ChangeState(player.MoveState);
    }

    public override void HandleDeath()
    {
        if (player.interactableObj is Documentation)
            player.interactableObj.OnInteract();
        base.HandleDeath();
    }
}