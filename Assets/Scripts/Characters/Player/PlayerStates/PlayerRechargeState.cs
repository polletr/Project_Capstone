using FMOD.Studio;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRechargeState : PlayerBaseState
{
    public PlayerRechargeState
        (PlayerController playerController) : base(playerController)
    {
    }

    private float progress; // Reference to the progress bar UI.
    private float ButtonMashBoost; // Extra progress per button press.
    private float maxTime; // Max value for the progress bar.


    public override void EnterState()
    {
        progress = 0;
        maxTime = Player.flashlight.MaxBatteryLife;
        ButtonMashBoost = Player.Settings.FlashlightReloadTime;
        // playerAnimator.animator.Play(playerAnimator.DieHash);
        Player.ReloadAnimation = Player.StartCoroutine(ReloadAnimation());
        
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
        if(progress >= maxTime)
        {
            Player.ChangeState(Player.MoveState);
        }
        
        progress += Player.Settings.FlashlightReloadTime * Time.deltaTime;
    }
    

    private IEnumerator ReloadAnimation()
    {
        while (true)
        {
            Player.Event.SetTutorialText?.Invoke("Recharging: " + progress.ToString("F0") + "\n" +"Tap R to boost recharge");
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void HandleMovement(Vector2 dir) { }

    public override void HandleLookAround(Vector2 dir, InputDevice device)
    {
     
    }


    public override void HandleAttack(bool held) { }

    protected override void HandleFlashlightSphereCast() { }

    public override void HandleFlashlightPower() { }

    public override void HandleRecharge()
    {
        progress += ButtonMashBoost + Player.playerInventory.CrankCollected;
    }
}