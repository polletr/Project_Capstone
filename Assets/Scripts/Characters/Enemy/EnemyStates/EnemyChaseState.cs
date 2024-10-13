using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{
    private Vector3 teleportPosition;
    private float teleportTimer;
    private float timeInChaseState;

    public EnemyChaseState(EnemyClass enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    public override void EnterState()
    {
        Debug.Log("Chasing (Teleport Mode)");
        enemy.agent.ResetPath();
        timeInChaseState = 0f;
        teleportTimer = 0;
    }

    public override void ExitState()
    {
        timeInChaseState = 0f;
        teleportTimer = 0;
    }
    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        timeInChaseState += Time.deltaTime;

        teleportTimer += Time.deltaTime;
        if (enemy.playerCharacter != null)
        {

            // If within attack range, switch to attack state
            if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
            {
                enemy.ChangeState(enemy.AttackState);
            }
            else
            {
                // Check if we should teleport
                if (teleportTimer >= enemy.TeleportCooldown)
                {
                    TeleportTowardsPlayer();
                }
            }
            Vector3 directionToPlayer = enemy.playerCharacter.transform.position - enemy.transform.position;
            directionToPlayer.y = 0f; // Ignore the y-axis for rotation
            enemy.transform.rotation = Quaternion.LookRotation(directionToPlayer.normalized);
        }

        // If no player is detected, return to idle state
        if (enemy.playerCharacter == null)
        {
            enemy.ChangeState(enemy.IdleState);
        }

    }

    private void TeleportTowardsPlayer()
    {
        teleportTimer = 0f;

        // Calculate the interpolation factor (0 = start, 1 = max time in chase state)
        float t = Mathf.Clamp01(timeInChaseState / enemy.MaxChaseTime);

        float currentTeleportMultiplier = Mathf.Lerp(2f, 0.8f, t);

        // Calculate teleport position based on the lerped multiplier
        Vector3 directionToPlayer = (enemy.playerCharacter.transform.position - enemy.transform.position).normalized;
        Vector3 desiredTeleportPosition = enemy.playerCharacter.transform.position - directionToPlayer * enemy.AttackRange * currentTeleportMultiplier;

        // Set a reasonable maximum distance to search for a valid NavMesh position
        float maxNavMeshDistance = 2f; // Adjust based on the scale of your game

        // Check if the desired position is on the NavMesh or find the closest valid point
        if (NavMesh.SamplePosition(desiredTeleportPosition, out NavMeshHit hit, maxNavMeshDistance, NavMesh.AllAreas))
        {
            teleportPosition = hit.position;
            enemy.transform.position = teleportPosition;
        }
        else
        {
            Debug.LogWarning("Could not find a valid teleport position on the NavMesh.");
        }
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
