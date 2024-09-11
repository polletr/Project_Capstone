using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public override void EnterState()
    {
        enemy.animator.CrossFade(ChaseHash, crossFadeDuration);

        enemy.Event.OnSoundEmitted += OnSoundDetected;

        enemy.agent.speed = enemy.ChaseSpeed;

        if (chasePos != null )
            enemy.agent.SetDestination(chasePos);
        else
            enemy.ChangeState(new EnemyPatrolState());

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

        if (enemy.agent.destination != chasePos)
        {
            enemy.agent.SetDestination(chasePos);
        }


        if (enemy.playerCharacter != null)
        {
            if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.SightRange && enemy.playerCharacter.GetComponent<PlayerController>().Health > 0)
            {
                chasePos = enemy.playerCharacter.transform.position;
                if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
                {
                    enemy.agent.ResetPath();
                    //enemy.ChangeState(new EnemyAttackState());
                }
            }
            else
            {
                enemy.agent.ResetPath();
                enemy.PatrolCenterPos = enemy.transform.position;
                enemy.ChangeState(new EnemyIdleState());
            }

        }
        
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance && enemy.agent.hasPath && enemy.playerCharacter == null)
        {
            if (enemy.agent.velocity.sqrMagnitude == 0f)
            {
                enemy.ChangeState(new EnemyIdleState());
            }
        }


    }

    protected override void OnSoundDetected(Vector3 soundPosition, float soundRange)
    {
        float distance = Vector3.Distance(enemy.transform.position, soundPosition);

        if (distance <= soundRange * enemy.HearingMultiplier && enemy.playerCharacter == null)
        {
            chasePos = soundPosition;
        }

    }

    protected override void VisionDetection()
    {
        float detectionRadius = enemy.SightRange;
        float detectionAngle = enemy.SightAngle;

        Collider[] targetsInViewRadius = Physics.OverlapSphere(enemy.transform.position, detectionRadius);
        if (enemy.playerCharacter == null)
        {
            foreach (Collider target in targetsInViewRadius)
            {
                Vector3 directionToTarget = (target.transform.position - enemy.transform.position).normalized;

                // Check if the target is within the cone's angle
                if (Vector3.Angle(enemy.transform.forward, directionToTarget) < detectionAngle / 2)
                {
                    // Perform a raycast to ensure there are no obstacles
                    RaycastHit hit;
                    if (Physics.Raycast(enemy.transform.position, directionToTarget, out hit, detectionRadius))
                    {
                        if (hit.collider == target)
                        {
                            if (hit.collider.CompareTag("Player"))
                            {
                                enemy.playerCharacter = hit.collider.gameObject;
                                Debug.Log("See Player");
                            }
                        }
                    }
                }
            }
        }

    }

}
