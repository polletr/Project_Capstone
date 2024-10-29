using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseAttackState : NurseBaseState
{
    public NurseAttackState(NurseEnemy enemyClass, EnemyAnimator enemyAnim)
    : base(enemyClass, enemyAnim) { }

    public override void EnterState() 
    {
        enemy.agent.ResetPath();

        // Play reaction animation
        enemyAnimator.animator.CrossFade(enemyAnimator.AttackHash, enemyAnimator.animationCrossFade);

        enemy.Attack();
    }

    public override void ExitState() { }

    public override void StateFixedUpdate() { }

    public override void StateUpdate() 
    {

        //RotateToPlayer();
            
    }


}
