using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseIdleState : NurseBaseState
{
    public NurseIdleState(NurseEnemy enemyClass, EnemyAnimator enemyAnim)
    : base(enemyClass, enemyAnim) { }

    public override void EnterState() 
    {
        enemy.agent.ResetPath();
        enemyAnimator.animator.CrossFade(enemyAnimator.IdleHash, enemyAnimator.animationCrossFade);

        PLAYBACK_STATE playbackState;
        enemy.currentAudio.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            //Check Audio for nurse Idle
            //enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.ShadowIdle);
            //enemy.currentAudio.start();
        }

    }

    public override void ExitState() { }

    public override void StateFixedUpdate() { }

    public override void StateUpdate() 
    {
        if (enemy.playerCharacter != null)
            RotateToPlayer();
    }

}
