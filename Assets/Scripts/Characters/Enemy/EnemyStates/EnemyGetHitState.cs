using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyGetHitState : EnemyBaseState
{
    public EnemyGetHitState(EnemyClass enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemyAnimator.animator.Play(enemyAnimator.GetHitHash);
    }
    public override void ExitState()
    {

    }

    public override void StateUpdate()
    {

        AnimatorStateInfo stateInfo = enemyAnimator.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == enemyAnimator.GetHitHash) // Ensure this matches the animation state name
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                enemy.ChangeState(enemy.ChaseState);
            }
        }
    }


    public override void StateFixedUpdate()
    {

    }


    protected override void VisionDetection()
    {

    }

}
