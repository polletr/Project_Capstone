using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{

    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemy.animator.Play(AttackHash);
    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        AnimatorStateInfo stateInfo = enemy.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == AttackHash) // Ensure this matches the animation state name
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                if (Vector3.Distance(enemy.agent.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
                    enemy.playerCharacter.GetComponent<PlayerController>().GetDamaged(enemy.AttackDamage);

                enemy.ChangeState(new EnemyIdleState());

            }
        }
    }

    protected override void VisionDetection()
    {

    }

}
