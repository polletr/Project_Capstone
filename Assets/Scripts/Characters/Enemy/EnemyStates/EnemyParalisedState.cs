using UnityEngine;

public class EnemyParalisedState : EnemyBaseState
{
    public EnemyParalisedState(EnemyClass enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemyAnimator.animator.CrossFade(enemyAnimator.IdleHash, enemyAnimator.animationCrossFade);
    }
    public override void ExitState()
    {

    }

    public override void StateUpdate()
    {
        //ROtate enemy towards player
    }


    public override void StateFixedUpdate()
    {

    }


}
