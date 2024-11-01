using FMOD.Studio;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{
    private float teleportTimer;

    public EnemyChaseState(ShadowEnemy enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim)
    { }

    public override void EnterState()
    {
        if (enemy.playerCharacter == null || !enemy.playerCharacter.IsAlive())
        {
            enemy.ChangeState(enemy.IdleState);
        }

        if (enemyAnimator.animator != null)
            enemyAnimator.animator.CrossFade(enemyAnimator.IdleHash, enemyAnimator.animationCrossFade);

        enemy.agent.ResetPath();
        teleportTimer = 0;

        PLAYBACK_STATE playbackState;
        enemy.currentAudio.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.ShadowIdle);
            enemy.currentAudio.start();
        }

        Debug.Log("CHasing Player");
    }

    public override void ExitState()
    {
        teleportTimer = 0;
        enemy.BodyMaterial.SetFloat("_Transparency", 0.9f);
        enemy.EyeMaterial.SetFloat("_Transparency", 0.9f);
    }

    public override void StateFixedUpdate()
    {
    }

    public override void StateUpdate()
    {
        if (enemy.EnemyTeleporting)
            return;

        teleportTimer += Time.deltaTime;
        if (enemy.playerCharacter != null)
        {
            RotateToPlayer();

            // If within attack range, switch to attack state
            if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
            {
                enemy.ChangeState(enemy.AttackState);
            }
            else
            {
                // Check if we should teleport
                if (teleportTimer >= enemy.TeleportCooldown)
                {
                    TeleportTowardsPlayer();
                }
            }
        }

        // If no player is detected, return to idle state
        if (enemy.playerCharacter == null)
        {
            enemy.ChangeState(enemy.IdleState);
        }
    }

    public override void HandleChase()
    {
        
    }

    protected override void VisionDetection()
    {
        
    }
    private void TeleportTowardsPlayer()
    {
        teleportTimer = 0f;

        // Calculate teleport position based on the lerped multiplier
        Vector3 directionToPlayer = (enemy.playerCharacter.transform.position - enemy.transform.position).normalized;
        Vector3 desiredTeleportPosition =
            enemy.playerCharacter.transform.position - directionToPlayer * enemy.AttackRange * 0.8f;

        if (CheckPath(desiredTeleportPosition))
        {
            enemy.StartCoroutine(enemy.TeleportEnemyWithDelay(desiredTeleportPosition));
        }
        else
        {
            Debug.LogWarning("No valid path to the teleport position. Changing to Idle state.");
            enemy.ChangeState(enemy.IdleState);
        }
    }


}