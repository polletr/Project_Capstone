using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour, IDamageable, IStunnable
{
    public Vector3 PatrolCenterPos { get; set; }
    public Animator animator { get; set; }
    public PlayerController playerCharacter { get; set; }
    public bool CanGetHit { get; set; }
    public Door TargetDoor { get; set; }

    public float PatrolRange { get { return patrolRange; } }
    public float MaxIdleTime { get { return maxIdleTime; } }
    public float MinIdleTime { get { return minIdleTime; } }
    public float PatrolSpeed { get { return patrolSpeed; } }
    public float ChaseSpeed { get { return chaseSpeed; } }
    public float HearingMultiplier { get { return hearingMultiplier; } }
    public float SightRange { get { return sightRange; } }
    public float SightAngle { get { return sightAngle; } }
    public float AttackRange { get { return attackRange; } }
    public float AttackDamage { get { return attackDamage; } }
    public float AttackAntecipationTime { get { return attackAntecipationTime; } }
    public float AttackRecoveryTime { get { return attackRecoveryTime; } }


    [HideInInspector]
    public EnemyBaseState currentState;

    [HideInInspector]
    public NavMeshAgent agent;

    [SerializeField] private float patrolRange;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField, Range(0f, 2f)] private float hearingMultiplier = 1f;
    [SerializeField, Range(0.2f, 15f)] private float sightRange = 5f;
    [SerializeField, Range(20f, 90f)] private float sightAngle = 45f;
    [SerializeField, Range(0.5f, 3f)] private float attackRange = 1f;
    [SerializeField] private float attackDamage = 1;
    [SerializeField] private float attackAntecipationTime = 1;
    [SerializeField] private float attackRecoveryTime = 1;

    [SerializeField] private float health = 3;
    [field: SerializeField] public float AttackCooldown { get; private set; } 

    public GameEvent Event;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        PatrolCenterPos = transform.position;
        ChangeState(new EnemyIdleState());
    }

    private void FixedUpdate() => currentState?.StateFixedUpdate();
    private void Update() => currentState?.StateUpdate();

    public void GetDamaged(float attackDamage)
    {
        health -= attackDamage;
        if (health > 0f)
        {
            currentState?.HandleGetHit();
        }
        else
        {
            currentState?.HandleDeath();
        }

    }

    public void ApplyEffect()
    {
        currentState?.HandleGetHit();
    }

    public void CanGetIntoGetHitState(int check)
    {
        CanGetHit = check == 0? true : false;
    }

    public virtual void Attack()
    {
        if (Vector3.Distance(transform.position, playerCharacter.transform.position) <= AttackRange)
            playerCharacter.GetComponent<PlayerController>().GetDamaged(AttackDamage);
    }

    #region ChangeState
    public void ChangeState(EnemyBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.enemy = this;
        currentState.EnterState();
    }
    #endregion

}
