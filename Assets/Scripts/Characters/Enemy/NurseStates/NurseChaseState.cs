using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseChaseState : NurseBaseState
{
    public NurseChaseState(NurseEnemy enemyClass, EnemyAnimator enemyAnim)
    : base(enemyClass, enemyAnim) { }

    public override void EnterState() { }

    public override void ExitState() { }

    public override void StateFixedUpdate() { }

    public override void StateUpdate() { }

    public override void HandleRetreat()
    {
        enemy.ChangeState(enemy.RetreatState);
    }


}
