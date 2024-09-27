using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyBaseState : MonoBehaviour
{
    protected Vector3 chasePos;

    protected float time; // How long the enemy will stay in the idle state
    protected float timer; // Tracks the time spent in the idle state

    protected static readonly int IdleHash = Animator.StringToHash("Idle");
    protected static readonly int ChaseHash = Animator.StringToHash("Chase");
    protected static readonly int WalkHash = Animator.StringToHash("Walk");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int GetHitHash = Animator.StringToHash("GetHit");
    protected static readonly int DieHash = Animator.StringToHash("Die");

    protected const float crossFadeDuration = 0.2f;

    public EnemyClass enemy { get; set; }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate() { }
    public virtual void HandleAttack() { }
    public virtual void HandleDeath() 
    {
        enemy.ChangeState(new EnemyDeathState());
    }

    protected virtual void OnSoundDetected(Vector3 soundPosition, float soundRange)
    {
        float distance = Vector3.Distance(enemy.transform.position, soundPosition);
        if (distance <= soundRange * enemy.HearingMultiplier)
        {
            chasePos = soundPosition;
            enemy.ChangeState(new EnemyChaseState());
        }
    }

    public virtual void HandleGetHit()
    {
        if (enemy.CanGetHit)
            enemy.ChangeState(new EnemyGetHitState());
    }


    protected virtual void VisionDetection()
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
                                chasePos = target.transform.position;
                                enemy.playerCharacter = hit.collider.gameObject;
                                enemy.ChangeState(new EnemyChaseState());
                            }
                        }
                    }
                }
            }
        }
    }


}