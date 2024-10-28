using FMOD.Studio;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

public class PlayerRechargeState : PlayerBaseState
{
    public PlayerRechargeState
        (PlayerController playerController) : base(playerController) { }
    
    private CountdownTimer timer;
    private string reloadText = "Recharging";
    
    public override void EnterState()
    {
        // playerAnimator.animator.Play(playerAnimator.DieHash);
        Player.ReloadAnimation = Player.StartCoroutine(ReloadAnimation());
        
        timer = new CountdownTimer(Player.Settings.FlashlightReloadTime);
        timer.Start();

        Player.playerFootsteps.getPlaybackState(out var playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
        {
            Player.playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }

    }
    public override void ExitState()
    {
        Player.StopCoroutine(Player.ReloadAnimation);
        // player.PlayerCam.transform.parent = player.CameraHolder;
        Player.Event.OnFinishRecharge?.Invoke();
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
            Player.ChangeState(Player.MoveState);
        }

       
    }
    
    private IEnumerator ReloadAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            Player.Event.SetTutorialText?.Invoke(reloadText = reloadText == "Recharging . . ." ? "Recharging" : reloadText + " .");
        }
    }

    public override void HandleMovement(Vector2 dir)
    {

    }
    
    public override void HandleLookAround(Vector2 dir, InputDevice device)
    {

    }


    public override void HandleAttack(bool held)
    {

    }
    
    protected override void HandleFlashlightSphereCast()
    {

    }

    public override void HandleFlashlightPower()
    {

    }



}

