using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(ShadowEnemy enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    private bool anticipating; // Flag for the anticipation phase
    private float anticipationTimer;
    public override void EnterState()
    {
        enemy.agent.ResetPath();

        // Start with anticipation phase
        anticipating = true;

        anticipationTimer = 0f; // Timer for the anticipation phase

        PLAYBACK_STATE playbackState;
        enemy.currentAudio.getPlaybackState(out playbackState);

    }

    public override void ExitState()
    {
        enemy.currentAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public override void StateFixedUpdate() { }

    public override void StateUpdate()
    {
        anticipationTimer += Time.deltaTime;

        RotateToPlayer();

        // Anticipation phase
        if (anticipating && enemy.playerCharacter.IsAlive())
        {

            //Eyes LErping to Red
            //Body Lerping to Red
            if (anticipationTimer >= enemy.AttackAntecipationTime) // End of anticipation phase
            {
                anticipating = false;

                // Check if player is within attack range
                float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position);
                if (distanceToPlayer <= enemy.AttackRange  && enemy.playerCharacter.IsAlive()) // If player is still in range
                {
                    // Stop anticipation sound and play attack sound
                    enemy.currentAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.Attack);
                    enemy.currentAudio.start();

                    enemy.Attack();
                    // Proceed with attack
                    enemy.enemyAnimator.animator.Play(enemyAnimator.AttackHash);

                    //enemy.ChangeState(enemy.IdleState);

                }
                else
                {
                    // Player moved out of range, return to chase state
                    enemy.ChangeState(enemy.IdleState);
                }
            }
        }
    }

    protected override void VisionDetection() { }
}