using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyGetHitState : EnemyBaseState
{

    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemy.animator.Play(GetHitHash);


    }
    public override void ExitState()
    {

    }

    public override void StateUpdate()
    {

        AnimatorStateInfo stateInfo = enemy.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == GetHitHash) // Ensure this matches the animation state name
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                enemy.ChangeState(new EnemyChaseState());
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
