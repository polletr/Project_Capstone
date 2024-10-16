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

        if (enemy.playerCharacter == null || !enemy.playerCharacter.IsAlive())
        {
            enemy.ChangeState(enemy.IdleState);
        }

        enemyAnimator.animator.CrossFade(enemyAnimator.IdleHash, enemyAnimator.animationCrossFade);

        enemy.agent.ResetPath();
        timeInChaseState = 0f;
        teleportTimer = 0;


    }

    public override void ExitState()
    {
        timeInChaseState = 0f;
        teleportTimer = 0;
        enemy.BodyMaterial.SetFloat("_Transparency", 0.9f);
        enemy.EyeMaterial.SetFloat("_Transparency", 0.9f);

    }
    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        if (enemy.EnemyTeleporting)
            return;

        timeInChaseState += Time.deltaTime;

        teleportTimer += Time.deltaTime;
        if (enemy.playerCharacter != null)
        {
            // Get the direction to the player
            Vector3 directionToPlayer = (enemy.playerCharacter.transform.position - enemy.transform.position).normalized;

            // Calculate the rotation towards the player
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z)); // Ignore y-axis to keep rotation flat

            // Smoothly rotate the enemy towards the player
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * enemy.RotationSpeed);

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

        float currentTeleportMultiplier = Mathf.Lerp(3f, 0.8f, t);

        // Calculate teleport position based on the lerped multiplier
        Vector3 directionToPlayer = (enemy.playerCharacter.transform.position - enemy.transform.position).normalized;
        Vector3 desiredTeleportPosition = enemy.playerCharacter.transform.position - directionToPlayer * enemy.AttackRange * currentTeleportMultiplier;

        if (CheckPath(desiredTeleportPosition))
        {
            enemy.StartCoroutine(enemy.TeleportEnemyWithDelay(desiredTeleportPosition));
        }
        else
        {
            Debug.LogWarning("No valid path to the teleport position. Changing to Idle state.");
            enemy.ChangeState(enemy.IdleState);
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
