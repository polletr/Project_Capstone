using FMOD.Studio;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRechargeState : PlayerBaseState
{
    public PlayerRechargeState
        (PlayerController playerController, Transform targetRotation) : base(playerController)
    {
    }

    private float progress; // Reference to the progress bar UI.
    private float lerpSpeed;
    private float ButtonMashBoost; // Extra progress per button press.
    private float maxTime; // Max value for the progress bar.

    Transform originalRotation;
    public override void EnterState()
    {
        progress = 0;
        lerpSpeed = 2f;
        maxTime = Player.flashlight.MaxBatteryLife;
        ButtonMashBoost = Player.Settings.FlashlightReloadTime;

       // Player.playerAnimator.animator.enabled = true;
       originalRotation = Player.PlayerCam.transform;

        Player.playerAnimator.animator.Play(Player.playerAnimator.RechargeHash);
        Debug.Log("Recharge state");

        // Player.playerAnimator.animator.CrossFade(Player.playerAnimator.RechargeHash, 0.5f);
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

        Player.Event.OnFinishRecharge?.Invoke();
    }

    public override void StateFixedUpdate()
    {
    }

    public override void StateUpdate()
    {
        if (progress >= maxTime)
        {
            Player.playerAnimator.animator.Play(Player.playerAnimator.IdleHash);

            Player.StartCoroutine(ReturnCam());
        }

        progress += Player.Settings.FlashlightReloadTime * Time.deltaTime;

        if (Player.playerAnimator.animator.speed > 1f)
        {
            // Lerp the speed back to 1 slowly
            Player.playerAnimator.animator.speed =
                Mathf.Lerp(Player.playerAnimator.animator.speed, 1f, lerpSpeed * Time.deltaTime);
        }


        var targetRotation = Quaternion.Euler(50, 0, 0); // Target rotation as a Quaternion (local space)
        var currentRotation = Player.PlayerCam.transform.localRotation;

// Check if the rotation is close to the target (prevent jitter)
        if (Quaternion.Angle(currentRotation, targetRotation) < 0.01f) return;

// Spherically interpolate towards the target rotation
        Player.PlayerCam.transform.localRotation = Quaternion.Slerp(currentRotation, targetRotation, lerpSpeed * Time.deltaTime);
        
        

    }


    private IEnumerator ReloadAnimation()
    {
        while (true)
        {
            Player.Event.SetTutorialText?.Invoke("Recharging: " + progress.ToString("F0") + "\n" +
                                                 "Tap R to boost recharge");
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator BackIdle()
    {
        while (Player.playerAnimator.enabled)
        {
            yield return new WaitForSeconds(0.1f);
            Player.playerAnimator.animator.enabled = false;
        }
    }

    private IEnumerator ReturnCam()
    {
        float threshold = 0.01f; // Angle threshold to stop rotating
        while (Quaternion.Angle(Player.PlayerCam.transform.localRotation, originalRotation.localRotation) > threshold)
        {
            // Smoothly interpolate back to the original rotation
            Player.PlayerCam.transform.localRotation = Quaternion.Slerp(
                Player.PlayerCam.transform.localRotation,
                originalRotation.localRotation,
                0.1f * Time.deltaTime);

            yield return null; // Wait for the next frame
        }

        // Ensure the rotation matches exactly at the end
        Player.PlayerCam.transform.localRotation = originalRotation.localRotation;

        // Transition to the next state
        Player.ChangeState(Player.MoveState);
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

    public override void HandleRecharge()
    {
        progress += ButtonMashBoost + Player.playerInventory.CrankCollected;
        Player.playerAnimator.animator.speed = 5f;
    }
}