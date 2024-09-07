using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState
{
    private Coroutine patrolRoutine;

    public override void EnterState()
    {
        patrolRoutine = enemy.StartCoroutine(Patrol());
    }

    public override void ExitState()
    {
        if (patrolRoutine != null)
        {
            enemy.StopCoroutine(patrolRoutine);
        }
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void StateUpdate()
    {
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance && enemy.agent.hasPath)
        {
            enemy.ChangeState(new EnemyIdleState());
        }
    }

    private IEnumerator Patrol()
    {
        Vector3 point;
        while (!enemy.agent.hasPath)
        {
            if (RandomPoint(enemy.StartPos, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                enemy.agent.SetDestination(point);
            }
            yield return null; // Wait until the next frame to avoid blocking
        }
    }

    private bool RandomPoint(Vector3 center, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * enemy.PatrolRange;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}