using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyClass enemyClass,EnemyAnimator enemyAnim) 
        : base(enemyClass,enemyAnim) { }
    public override void EnterState()
    {
        enemyAnimator.animator.CrossFade(enemyAnimator.WalkHash, enemyAnimator.animationCrossFade);

        enemy.agent.speed = enemy.PatrolSpeed;

        Debug.Log("Chasing");

        //enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.Chase);
        //enemy.currentAudio.start();

    }
    public override void ExitState()
    {
        //enemy.currentAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
            if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.SightRange && enemy.playerCharacter.GetComponent<PlayerController>().IsAlive())
            {
                chasePos = enemy.playerCharacter.transform.position;
                if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
                {
                    enemy.agent.ResetPath();
                    enemy.ChangeState(enemy.AttackState);
                }
            }

        }
        
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance && enemy.agent.hasPath && enemy.playerCharacter == null)
        {
            enemy.ChangeState(enemy.IdleState);
        }


    }


    protected override void OnSoundDetected(Vector3 soundPosition, float soundRange)
    {

    }


    protected override void VisionDetection()
    {
        float detectionRadius = enemy.SightRange;
        float detectionAngle = enemy.SightAngle;

        Collider[] targetsInViewRadius = Physics.OverlapSphere(enemy.transform.position, detectionRadius);

        foreach (Collider target in targetsInViewRadius)
        {
            Vector3 directionToTarget = (target.transform.position - enemy.transform.position).normalized;

            // Check if the target is within the cone's angle
            if (Vector3.Angle(enemy.transform.forward, directionToTarget) < detectionAngle / 2)
            {
                if (Physics.Raycast(enemy.transform.position, directionToTarget, out RaycastHit hit, detectionRadius))
                {
                    if (hit.collider == target)
                    {
                        if (hit.collider.CompareTag("Player") && enemy.playerCharacter == null)
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
