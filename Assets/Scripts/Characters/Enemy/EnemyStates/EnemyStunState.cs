using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class EnemyStunState : EnemyBaseState
{
    public EnemyStunState(EnemyClass enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    CountdownTimer countdownTimer;


    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemyAnimator.animator.Play(enemyAnimator.StunHash);

        countdownTimer = new CountdownTimer(enemy.StunTime);
        countdownTimer.Start();
        enemy.StopAllCoroutines();
        enemy.StartCoroutine(enemy.EnemyTransparency(0f));
        enemy.SmokeParticle.Play();
        enemy.EnemyCollider.enabled = false;
        enemy.currentAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }
    public override void ExitState()
    {
        enemy.StopAllCoroutines();
        enemy.StartCoroutine(enemy.EnemyTransparency(0.9f));
        enemy.SmokeParticle.Play();
        enemy.EnemyCollider.enabled = true;
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        countdownTimer.Tick(Time.deltaTime);
        if (countdownTimer.IsFinished)
        {
            Debug.Log("Back to Idle");
            enemy.ChangeState(enemy.IdleState);
        }

    }

    protected override void OnSoundDetected(Vector3 soundPosition, float soundRange)
    {

    }

    protected override void VisionDetection()
    {

    }

    public override void HandleParalise()
    {

    }

    public override void HandleChase()
    {
        
    }



}
