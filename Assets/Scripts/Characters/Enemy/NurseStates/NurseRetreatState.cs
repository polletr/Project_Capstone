using FMOD.Studio;
using UnityEngine;
using System.Collections;

public class NurseRetreatState : NurseBaseState
{
    private Vector3 targetPos;
    private bool isRetreating;

    public NurseRetreatState(NurseEnemy enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    public override void EnterState()
    {
        enemy.ChasePlayer = false;
        targetPos = enemy.PatrolTransforms[enemy.CurrentIdleSpotIndex].transform.position;

        // Check if a valid path to target exists
        if (!CheckPath(targetPos))
        {
            enemy.ChangeState(enemy.IdleState);
            Debug.LogError("Nurse can't find transform destination");
            return;
        }

        // Reset path and speed
        enemy.agent.ResetPath();
        enemy.agent.speed = 0;  // Pause movement during reaction

        // Play reaction animation
        enemyAnimator.animator.CrossFade(enemyAnimator.StunHash, enemyAnimator.animationCrossFade);

        // Play reaction audio if not already playing
        PLAYBACK_STATE playbackState;
        enemy.currentAudio.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            // enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents);
            // enemy.currentAudio.start();
        }

        // Start coroutine to wait for reaction animation to finish
        enemy.StartCoroutine(WaitForReactionAnimation());
    }

    private IEnumerator WaitForReactionAnimation()
    {
        yield return new WaitForSeconds(1.2f);
        StartRetreatMovement();
    }

    private void StartRetreatMovement()
    {
        isRetreating = true;
        enemy.agent.speed = enemy.RetreatSpeed;
        enemy.agent.SetDestination(targetPos);
        enemyAnimator.animator.CrossFade(enemyAnimator.WalkHash, enemyAnimator.animationCrossFade);
    }

    public override void StateUpdate()
    {
        if (isRetreating && Vector3.Distance(enemy.transform.position, targetPos) <= 0.1f)
        {
            enemy.ChangeState(enemy.IdleState);
        }
    }

    public override void ExitState()
    {
        enemy.agent.ResetPath();  // Clear the agent path on exit
        isRetreating = false;  // Reset retreat flag
    }

    public override void StateFixedUpdate() { }

    public override void HandleRetreat()
    {
        
    }
}