using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{
    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemy.animator.CrossFade(DieHash, crossFadeDuration);

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

    public override void HandleGetHit()
    {

    }

    public override void HandleDeath()
    {

    }


}
