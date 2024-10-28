using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState
{
    
    public EnemyPatrolState(ShadowEnemy enemyClass,EnemyAnimator enemyAnim) 
        : base(enemyClass,enemyAnim) { }
    
    private Coroutine patrolRoutine;
    private int currentPatrolIndex = 0;

    public override void EnterState()
    {
        enemyAnimator.animator.CrossFade(enemyAnimator.WalkHash, enemyAnimator.animationCrossFade);

        enemy.Event.OnSoundEmitted += OnSoundDetected;

        enemy.agent.speed = enemy.PatrolSpeed;
        patrolRoutine = enemy.StartCoroutine(Patrol());
        enemy.playerCharacter = null;

    }

    public override void ExitState()
    {

        enemy.Event.OnSoundEmitted -= OnSoundDetected;

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
        VisionDetection();
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance && enemy.agent.hasPath)
        {
            enemy.ChangeState(enemy.IdleState);
        }
    }

    private IEnumerator Patrol()
    {
        Vector3 point;
        while (!enemy.agent.hasPath)
        {
            if (enemy.PatrolTransforms.Count > 1) // Patrol between preset points
            {
                // Get the next patrol point (loop back to the start if at the end)
                Transform patrolPoint = enemy.PatrolTransforms[currentPatrolIndex];
                enemy.agent.SetDestination(patrolPoint.position);
                Debug.DrawRay(patrolPoint.position, Vector3.up, Color.green, 1.0f);

                // Move to the next point in the list, looping around
                currentPatrolIndex = (currentPatrolIndex + 1) % enemy.PatrolTransforms.Count;
            }
            else
            {
                if (RandomPoint(enemy.PatrolCenterPos, out point))
                {
                    Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                    enemy.agent.SetDestination(point);
                }
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