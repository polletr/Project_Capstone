using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{

    public override void EnterState()
    {
        Debug.Log("Enter Idle");
        enemy.agent.ResetPath();
        enemy.animator.CrossFade(IdleHash, crossFadeDuration);

        enemy.Event.OnSoundEmitted += OnSoundDetected;
        // Randomize the idle time between a range of seconds
        idleTime = Random.Range(enemy.MinIdleTime, enemy.MaxIdleTime);
        idleTimer = 0f; // Reset the timer
        enemy.playerCharacter = null;

    }
    public override void ExitState()
    {
        enemy.Event.OnSoundEmitted -= OnSoundDetected;
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        VisionDetection();

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTime)
        {
            enemy.ChangeState(new EnemyPatrolState());
        }
    }

}
