using FMOD.Studio;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState
        (PlayerController playerController) : base(playerController) { }


    public Transform EnemyFace { get; set; }
    public EnemyClass EnemyKiller { get; set; }

    CountdownTimer blackscreenTimer;
    CountdownTimer waitTimer;
    float FOV;
    bool hasFaded;
    public override void EnterState()
    {
        FOV = Player.PlayerCam.fieldOfView;
        Player.Event.OnPlayerDeath?.Invoke();
        //playerAnimator.animator.Play(playerAnimator.DieHash);

        Player.playerFootsteps.getPlaybackState(out var playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
        {
            Player.playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }

        blackscreenTimer = new CountdownTimer(3f);
        blackscreenTimer.Start();

        waitTimer = new CountdownTimer(0.5f);
        waitTimer.Start();

        hasFaded = false;
    }
    public override void ExitState()
    {
        // player.PlayerCam.transform.parent = player.CameraHolder;
        Player.PlayerCam.fieldOfView = FOV;
        EnemyFace = null;
        EnemyKiller = null;
        hasFaded = false;
        Debug.Log("Exit DeathState");
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        blackscreenTimer.Tick(Time.deltaTime);
        waitTimer.Tick(Time.deltaTime);

        if (hasFaded) return;

        if (IsKilledByEnemy() && waitTimer.IsFinished)
        {
            // Smoothly rotate the camera to look at the enemy's face
            Vector3 direction = (EnemyFace.position - Player.PlayerCam.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Player.PlayerCam.transform.rotation = Quaternion.Slerp(Player.PlayerCam.transform.rotation, lookRotation, 7 * Time.deltaTime);

            Player.PlayerCam.fieldOfView = Mathf.Lerp(Player.PlayerCam.fieldOfView, 25, 7 * Time.deltaTime);
            if (blackscreenTimer.IsFinished)
            {
                Player.Event.OnFadeBlackScreen?.Invoke();
                Debug.Log("Blackscreen start");
                hasFaded = true;
            }
        }      
    }

    public override void HandleMovement(Vector2 dir)
    {

    }

    public override void HandleLookAround(Vector2 dir, InputDevice device)
    {

    }

    public override void HandleDeath()
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

    private bool IsKilledByEnemy()
    {
        return EnemyKiller != null && EnemyFace != null;
    }
}
