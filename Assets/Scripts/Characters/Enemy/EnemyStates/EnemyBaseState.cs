using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class EnemyBaseState 
{
    protected Vector3 chasePos;

    protected float time; // How long the enemy will stay in the idle state
    protected float timer; // Tracks the time spent in the idle state

    protected const float crossFadeDuration = 0.2f;

    public EnemyAnimator enemyAnimator { get; private set; }
    public EnemyClass enemy { get; set; }
    
    public EnemyBaseState(EnemyClass enemyClass,EnemyAnimator enemyAnim)
    {
        enemy = enemyClass;
        enemyAnimator = enemyAnim;
    }
    

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate() { }
    public virtual void HandleAttack() { }
    public virtual void HandleDeath() 
    {
        enemy.ChangeState(enemy.DeathState);
    }

    protected virtual void OnSoundDetected(Vector3 soundPosition, float soundRange)
    {
        float distance = Vector3.Distance(enemy.transform.position, soundPosition);
        if (distance <= soundRange * enemy.HearingMultiplier)
        {
            chasePos = soundPosition;
            enemy.ChangeState(enemy.ChaseState);
        }
    }

    public virtual void HandleGetHit()
    {
        if (enemy.CanGetHit)
            enemy.ChangeState(enemy.GetHitState);
    }


    protected virtual void VisionDetection()
    {
        float detectionRadius = enemy.SightRange;
        float detectionAngle = enemy.SightAngle;

        Collider[] targetsInViewRadius = Physics.OverlapSphere(enemy.transform.position, detectionRadius);
        if (enemy.playerCharacter != null)
        {
            return;
        }

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
                            Debug.Log("See Player");
                            chasePos = target.transform.position;
                            enemy.playerCharacter = hit.collider.GetComponent<PlayerController>();
                            enemy.playerCharacter.AddEnemyToChaseList(enemy);
                            enemy.ChangeState(enemy.ChaseState);
                        }
                    }
                }
            }
        }

    }


}