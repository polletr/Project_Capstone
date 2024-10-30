using UnityEngine;
using UnityEngine.AI;
using Utilities;

public class EnemyParalisedState : EnemyBaseState
{
    CountdownTimer countTimer;
    public EnemyParalisedState(ShadowEnemy enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    public override void EnterState()
    {
        enemy.Paralised = true;
        enemy.agent.ResetPath();

        enemy.agent.speed = enemy.ParalisedSpeed;

        enemyAnimator.animator.CrossFade(enemyAnimator.IdleHash, enemyAnimator.animationCrossFade);

        countTimer = new CountdownTimer(5f);
        countTimer.Start();
    }
    public override void ExitState()
    {
        enemy.Paralised = false;
    }

    public override void StateUpdate()
    {
        countTimer.Tick(Time.deltaTime);
        if (enemy.playerCharacter == null)
        {
            enemy.playerCharacter = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            enemy.playerCharacter.AddEnemyToChaseList(enemy);
        }

        if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
        {
            enemy.ChangeState(enemy.AttackState);
        }

        // Get the direction to the player
        Vector3 directionToPlayer = (enemy.playerCharacter.transform.position - enemy.transform.position).normalized;

        // Calculate the rotation towards the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z)); // Ignore y-axis to keep rotation flat

        // Smoothly rotate the enemy towards the player
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * enemy.RotationSpeed);

        if (countTimer.IsFinished)
        {
            TeleportToRandomPositionAroundPlayer();
        }
    }

    void TeleportToRandomPositionAroundPlayer()
    {
        Vector3 targetPosition = Vector3.zero;
        Debug.Log("Teleport");

        while (targetPosition == Vector3.zero)
        {
            // Generate a random direction and distance within the specified radius range
            Vector3 randomDirection = Random.insideUnitSphere * 5f;
            randomDirection += enemy.playerCharacter.transform.position;
            // Ensure it's not closer than the minimum radius
            if (Vector3.Distance(randomDirection, enemy.playerCharacter.transform.position) >= enemy.AttackRange * 1.5f)
            {
                // If CheckPath approves, set the position as the target
                if (CheckPath(randomDirection))
                {
                    targetPosition = randomDirection;
                }
            }
        }

        enemy.StartCoroutine(enemy.TeleportEnemyWithDelay(targetPosition));
        countTimer.Reset();
    }

    public override void StateFixedUpdate()
    {

    }

    protected override void VisionDetection() { }

    protected override void OnSoundDetected(Vector3 soundPosition, float soundRange) { }

    public override void HandleParalise()
    {

    }




}
