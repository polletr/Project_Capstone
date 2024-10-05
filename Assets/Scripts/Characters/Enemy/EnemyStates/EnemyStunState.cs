using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunState : EnemyBaseState
{
    public EnemyStunState(EnemyClass enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }
    
    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemyAnimator.animator.CrossFade(enemyAnimator.StunHash, enemyAnimator.animationCrossFade);

    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {

    }

    protected override void OnSoundDetected(Vector3 soundPosition, float soundRange)
    {

    }

    protected override void VisionDetection()
    {

    }


}
