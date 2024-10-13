using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

public class PlayerRechargeState : PlayerBaseState
{
    public PlayerRechargeState
        (PlayerController playerController) : base(playerController) { }
    
    CountdownTimer timer;

    public override void EnterState()
    {
        // playerAnimator.animator.Play(playerAnimator.DieHash);

        timer = new CountdownTimer(player.Settings.FlashlightReloadTime);
        timer.Start();
        
        Debug.Log("Recharging");
        
    }
    public override void ExitState()
    {
        // player.PlayerCam.transform.parent = player.CameraHolder;
        player.Event.OnFinishRecharge?.Invoke();

    }

    public override void StateFixedUpdate()
    {
      
    }

    public override void StateUpdate()
    {
        timer.Tick(Time.deltaTime);
        if (timer.IsFinished)
        {
            //animation stuff here
            player.ChangeState(player.MoveState);
        }
    }

    public override void HandleMovement(Vector2 dir)
    {

    }
    
    public override void HandleLookAround(Vector2 dir, InputDevice device)
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

