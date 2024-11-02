using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NurseBaseState
{
    protected Vector3 chasePos;

    protected float time;
    protected float timer;

    public EnemyAnimator enemyAnimator { get; private set; }
    public NurseEnemy enemy { get; set; }

    public NurseBaseState(NurseEnemy enemyClass, EnemyAnimator enemyAnim)
    {
        enemy = enemyClass;
        enemyAnimator = enemyAnim;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate() { }
    public virtual void HandleAttack() 
    {
        enemy.ChangeState(enemy.AttackState);
    }

    public virtual void HandleRetreat() 
    {

    }
    protected virtual void RotateToPlayer()
    {
        // Get the direction to the player, ignoring the Y-axis
        Vector3 directionToPlayer = enemy.playerCharacter.transform.position - enemy.transform.position;
        directionToPlayer.y = 0;  // Zero out the Y-axis to keep rotation flat

        // Ensure direction is non-zero to avoid errors
        if (directionToPlayer != Vector3.zero)
        {
            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Smoothly rotate the enemy towards the player with the offset
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                targetRotation,
                Time.deltaTime * enemy.RotationSpeed
            );
        }
    }
    protected bool CheckPath(Vector3 targetPos)
    {
        // Check if the desired position is on the NavMesh or find the closest valid point
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            // Create a new path
            NavMeshPath path = new NavMeshPath();

            // Calculate the path from the enemy's current position to the desired teleport position
            if (NavMesh.CalculatePath(enemy.transform.position, hit.position, NavMesh.AllAreas, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return true;
                }

            }
        }

        return false;
    }

}
