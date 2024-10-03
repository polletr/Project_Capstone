using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOpenDoorState : EnemyBaseState
{
    private bool attacked;
    private float attackCooldown; // Time between attacks
    private float attackCooldownTimer;
    
    public EnemyOpenDoorState(EnemyClass enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    public override void EnterState()
    {
        Debug.Log("Open Door");
        enemy.agent.ResetPath();
        attacked = false;
        enemy.Event.OnSoundEmitted += OnSoundDetected;

        time = enemy.AttackAntecipationTime;
        attackCooldown = enemy.AttackCooldown;
        attackCooldownTimer = 0f;
        timer = 0f;

        enemy.agent.SetDestination(enemy.TargetDoor.transform.position);

        enemyAnimator.animator.Play(enemyAnimator.AttackHash);

    }

    public override void ExitState() 
    {
        enemy.Event.OnSoundEmitted -= OnSoundDetected;
    }

    public override void StateFixedUpdate() { }

    public override void StateUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= time && !attacked)
        {
            AttackDoor();
            attacked = true;
        }

        // If attacked, wait for the cooldown to finish before attacking again
        if (attacked)
        {
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer >= attackCooldown)
            {
                // Reset for the next attack
                attacked = false;
                attackCooldownTimer = 0f;
                timer = 0f; // Reset the anticipation timer for next attack
                enemyAnimator.animator.Play(enemyAnimator.AttackHash);
            }
        }


        if (enemy.playerCharacter != null)
        {
            if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.SightRange && enemy.playerCharacter.GetComponent<PlayerController>().IsAlive())
            {
                chasePos = enemy.playerCharacter.transform.position;
            }
            else
            {
                enemy.agent.ResetPath();
                enemy.PatrolCenterPos = enemy.transform.position;
                enemy.ChangeState(enemy.IdleState);
            }

        }
        else
        {
            enemy.ChangeState(enemy.IdleState);
        }

        // Check if the door is opened
        if (enemy.TargetDoor.IsOpen)
        {
            enemy.ChangeState(enemy.ChaseState); // Go to another state once the door is open
        }

    }

    protected override void OnSoundDetected(Vector3 soundPosition, float soundRange)
    {
        float distance = Vector3.Distance(enemy.transform.position, soundPosition);
        if (distance <= soundRange * enemy.HearingMultiplier / 2f && enemy.agent.hasPath)
        {
            chasePos = soundPosition;
            enemy.ChangeState(enemy.ChaseState);
        }
    }


    private void AttackDoor()
    {
        // Logic for applying damage or force to the door
        enemy.TargetDoor.TakeDamage(enemy.AttackDamage, enemy.gameObject);
    }

    protected override void VisionDetection()
    {

    }
}