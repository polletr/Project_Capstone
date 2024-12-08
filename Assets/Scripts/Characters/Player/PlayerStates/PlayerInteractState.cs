using FMOD.Studio;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractState : PlayerBaseState
{
    public PlayerInteractState(PlayerController playerController) : base(playerController)
    {
    }

    public override void EnterState()
    {
        Player.interactableObj.OnInteract();

        if (Player.interactableObj is not Documentation)
            Player.ChangeState(Player.MoveState);

        Player.playerFootsteps.getPlaybackState(out var playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
        {
            Player.playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    public override void ExitState()
    {
        Player.interactableObj = null;
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
        Player.flashlight.ZeroOutBattery();
        Player.ChangeState(Player.RechargeState);
    }

    public override void HandleAttack(bool held)
    {
        
    }

    public override void HandleFlashlightPower()
    {
      
    }

    public override void HandleInteract()
    {
        if (Player.interactableObj is not Documentation) return;

        Player.interactableObj.OnInteract();
        Player.ChangeState(Player.MoveState);
    }

    public override void HandleDeath()
    {
        if (Player.interactableObj is Documentation)
            Player.interactableObj.OnInteract();
        base.HandleDeath();
    }
}