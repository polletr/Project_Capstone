using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    
    public EnemyIdleState(EnemyClass enemyClass, EnemyAnimator enemyAnim) 
        : base(enemyClass,enemyAnim) { }

    public override void EnterState()
    {
        Debug.Log("Idle State");
        enemy.agent.ResetPath();
        enemyAnimator.animator.CrossFade(enemyAnimator.IdleHash, crossFadeDuration);

        enemy.Event.OnSoundEmitted += OnSoundDetected;
        // Randomize the idle time between a range of seconds
        time = Random.Range(enemy.MinIdleTime, enemy.MaxIdleTime);
        timer = 0f; // Reset the timer
        if (enemy.playerCharacter != null)
        {
            enemy.playerCharacter.RemoveEnemyFromChaseList(enemy);
            enemy.playerCharacter = null;
        }

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

        timer += Time.deltaTime;

        if (timer >= time)
        {
            enemy.ChangeState(enemy.PatrolState);
        }
    }

}
