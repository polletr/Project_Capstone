using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private bool attacked;
    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemy.animator.Play(AttackHash);

        attacked = false;

        time = enemy.AttackTimeInterval;
        timer = 0f; // Reset the timer

    }
    public override void ExitState()
    {

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
            if (Vector3.Distance(enemy.agent.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
                enemy.playerCharacter.GetComponent<PlayerController>().GetDamaged(enemy.AttackDamage);
        }

        AnimatorStateInfo stateInfo = enemy.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == AttackHash) // Ensure this matches the animation state name
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                enemy.ChangeState(new EnemyChaseState());
            }
        }
    }

    protected override void VisionDetection()
    {

    }

}
