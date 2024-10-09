using UnityEngine;
using Utilities;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState
        (PlayerController playerController) : base(playerController) { }


    public Transform EnemyFace { get; set; }
    public EnemyClass EnemyKiller { get; set; }

    CountdownTimer timer;

    public override void EnterState()
    {
        player.Event.OnPlayerDeath?.Invoke();
        playerAnimator.animator.Play(playerAnimator.DieHash);


        timer = new CountdownTimer(player.Settings.RespawnTime);
        timer.Start();


        if (IsKilledByEnemy())
        {
           player.PlayerCam.transform.LookAt(EnemyFace.position); 
        }
        else
        {
          //UI fade black or something
        }
    }
    public override void ExitState()
    {
       // player.PlayerCam.transform.parent = player.CameraHolder;
        player.Event.OnPlayerRespawn?.Invoke();

        EnemyFace = null;
        EnemyKiller = null;
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
    }

    public override void HandleMovement(Vector2 dir)
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
        return EnemyKiller != null && EnemyFace;
    }

}
