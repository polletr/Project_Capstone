using FMOD.Studio;
using UnityEngine;

public class NurseChaseState : NurseBaseState
{
    private Vector3 lastPlayerPosition;

    public NurseChaseState(NurseEnemy enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    public override void EnterState()
    {
        if (!CheckPath(enemy.playerCharacter.transform.position))
        {
            HandleRetreat();
            return;
        }

        enemy.agent.ResetPath();
        enemyAnimator.animator.CrossFade(enemyAnimator.ChaseHash, enemyAnimator.animationCrossFade);

        enemy.agent.speed = enemy.ChaseSpeed;
        lastPlayerPosition = enemy.playerCharacter.transform.position;  // Store initial player position

        // Start chase audio if not already playing
        PLAYBACK_STATE playbackState;
        enemy.currentAudio.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            // Play the chase audio if it's stopped
            // enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents);
            // enemy.currentAudio.start();
        }

        enemy.agent.SetDestination(lastPlayerPosition);  // Initial destination setup
    }

    public override void StateUpdate()
    {
        // Update destination only if player has moved
        if (Vector3.Distance(lastPlayerPosition, enemy.playerCharacter.transform.position) > 0.1f)
        {
            lastPlayerPosition = enemy.playerCharacter.transform.position;

            if (CheckPath(lastPlayerPosition))
            {
                enemy.agent.SetDestination(lastPlayerPosition);
            }
            else
            {
                HandleRetreat();
            }
        }

        if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
        {
            HandleAttack();
        }
    }

    public override void ExitState()
    {
        enemy.agent.ResetPath();  // Clear the agent path on exit
    }

    public override void StateFixedUpdate() { }

    public override void HandleRetreat()
    {
        enemy.ChangeState(enemy.RetreatState);
    }
}