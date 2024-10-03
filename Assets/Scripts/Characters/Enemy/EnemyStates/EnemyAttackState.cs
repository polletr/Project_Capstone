using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    
    public EnemyAttackState(EnemyClass enemyClass,EnemyAnimator enemyAnim) 
        : base(enemyClass,enemyAnim) { }
    
    private bool attacked;
    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemy.enemyAnimator.animator.Play(enemyAnimator.AttackHash);

        attacked = false;

        time = enemy.AttackAntecipationTime;
        timer = 0f; // Reset the timer

        enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.Attack);
        enemy.currentAudio.start();


    }
    public override void ExitState()
    {
        enemy.currentAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= time && !attacked)
        {
            attacked = true;
        }


        AnimatorStateInfo stateInfo = enemyAnimator.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == enemyAnimator.AttackHash) // Ensure this matches the animation state name
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                enemy.ChangeState(enemy.ChaseState);
            }
        }
    }

    protected override void VisionDetection()
    {

    }

}
