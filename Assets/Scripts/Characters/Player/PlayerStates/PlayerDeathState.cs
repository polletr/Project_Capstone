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

    public override void EnterState()
    {
        FOV = player.PlayerCam.fieldOfView;
        player.Event.OnPlayerDeath?.Invoke();
        //playerAnimator.animator.Play(playerAnimator.DieHash);

        Debug.Log("Player is dead");

        timer = new CountdownTimer(player.Settings.RespawnTime);
        timer.Start();

    }
    public override void ExitState()
    {
        // player.PlayerCam.transform.parent = player.CameraHolder;
        player.PlayerCam.fieldOfView = FOV;
        player.transform.position = player.CheckPoint.position;
        EnemyFace = null;
        EnemyKiller = null;

        Debug.Log("Player is alive");
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
            player.Event.OnPlayerRespawn?.Invoke();
        }


        if (IsKilledByEnemy())
        {
            // Smoothly rotate the camera to look at the enemy's face
            Vector3 direction = (EnemyFace.position - player.PlayerCam.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            player.PlayerCam.transform.rotation = Quaternion.Slerp(player.PlayerCam.transform.rotation, lookRotation, 7 * Time.deltaTime);

            player.PlayerCam.fieldOfView = Mathf.Lerp(player.PlayerCam.fieldOfView, 25, 7 * Time.deltaTime);

            // Trigger UI fade or effect here      
            //UI fade black or something
        }
        else
        {
            //UI fade black or something
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

    public override void HandleFlashlightSphereCast()
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
