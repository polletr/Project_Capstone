using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float idleTime; // How long the enemy will stay in the idle state
    private float idleTimer; // Tracks the time spent in the idle state

    public override void EnterState()
    {
        // Randomize the idle time between a range of seconds
        idleTime = Random.Range(enemy.MinIdleTime, enemy.MaxIdleTime);
        idleTimer = 0f; // Reset the timer
    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTime)
        {
            enemy.ChangeState(new EnemyPatrolState());
        }
    }
}
