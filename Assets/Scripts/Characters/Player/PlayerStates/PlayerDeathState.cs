using UnityEngine;
using Utilities;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState
        (PlayerAnimator animator, PlayerController playerController, InputManager inputM)
        : base(animator, playerController, inputM) { }

    CountdownTimer timer;

    public override void EnterState()
    {
        playerAnimator.animator.Play(playerAnimator.DieHash);
        player.Event.OnPlayerDeath?.Invoke();
        timer = new CountdownTimer(player.Settings.RespawnTime);
        timer.Start();
        player.PlayerCam.transform.parent = player.DeathParentObj;
    }
    public override void ExitState()
    {
        player.PlayerCam.transform.parent = player.CameraHolder;
        player.Event.OnPlayerRespawn?.Invoke();
    }

    public override void StateFixedUpdate()
    {
      
    }

    public override void StateUpdate()
    {
      timer.Tick(Time.deltaTime);
        if (timer.IsFinished)
        {
            player.Event.OnPlayerRespawn?.Invoke(); 
        }
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
