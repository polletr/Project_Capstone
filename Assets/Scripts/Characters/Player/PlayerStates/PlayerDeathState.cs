using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState
        (PlayerController playerController) : base(playerController) { }


    public Transform EnemyFace { get; set; }
    public EnemyClass EnemyKiller { get; set; }

    CountdownTimer timer;
    float FOV;
    bool hasFaded;
    public override void EnterState()
    {
        FOV = player.PlayerCam.fieldOfView;
        player.Event.OnPlayerDeath?.Invoke();
        //playerAnimator.animator.Play(playerAnimator.DieHash);

        timer = new CountdownTimer(3f);
        timer.Start();

    }
    public override void ExitState()
    {
        // player.PlayerCam.transform.parent = player.CameraHolder;
        player.PlayerCam.fieldOfView = FOV;
        player.transform.position = player.CheckPoint.position;
        EnemyFace = null;
        EnemyKiller = null;
        hasFaded = false;
        Debug.Log("Player is alive");
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        timer.Tick(Time.deltaTime);

        if (hasFaded) return;


  
        if (timer.IsFinished)
        {
            player.Event.OnFadeBlackScreen?.Invoke();
            Debug.Log("Blackscreen start");
            hasFaded = true;
        }

        if (IsKilledByEnemy())
        {
            // Smoothly rotate the camera to look at the enemy's face
            Vector3 direction = (EnemyFace.position - player.PlayerCam.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            player.PlayerCam.transform.rotation = Quaternion.Slerp(player.PlayerCam.transform.rotation, lookRotation, 7 * Time.deltaTime);

            player.PlayerCam.fieldOfView = Mathf.Lerp(player.PlayerCam.fieldOfView, 25, 7 * Time.deltaTime);
        }
        else
        {
            //UI fade black or something
            player.Event.OnFadeBlackScreen?.Invoke();
            hasFaded = true;

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

    public override void HandleAttack(bool isHeld)
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
