using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour, IStunnable
{
    public Vector3 PatrolCenterPos { get; set; }
    public PlayerController playerCharacter { get; set; }
    public bool Paralised { get; set; }
    public Door TargetDoor { get; set; }
    public EventInstance currentAudio{ get; set; }

    [SerializeField] private List<Transform> patrolTransforms = new List<Transform>();

    public List<Transform> PatrolTransforms
    {
        get { return patrolTransforms; }
        set { patrolTransforms = value; }
    }
    public float PatrolRange
    {
        get { return patrolRange; }
    }

    public float MaxIdleTime
    {
        get { return maxIdleTime; }
    }

    public float MinIdleTime
    {
        get { return minIdleTime; }
    }

    public float PatrolSpeed
    {
        get { return patrolSpeed; }
    }

    public float ParalisedSpeed
    {
        get { return paralisedSpeed; }
    }

    public float HearingMultiplier
    {
        get { return hearingMultiplier; }
    }

    public float SightRange
    {
        get { return sightRange; }
    }

    public float SightAngle
    {
        get { return sightAngle; }
    }

    public float AttackRange
    {
        get { return attackRange; }
    }

    public float RotationSpeed
    {
        get { return rotationSpeed; }
    }

    public float AttackAntecipationTime
    {
        get { return attackAntecipationTime; }
    }

    public EnemyAttackState AttackState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyStunState StunState { get; private set; }
    public EnemyParalisedState ParalisedState { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyOpenDoorState OpenDoorState { get; private set; }


    public EnemyBaseState currentState;
    public EnemyAnimator enemyAnimator { get; set; }

    [HideInInspector] public NavMeshAgent agent;

    [SerializeField] private float patrolRange;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float paralisedSpeed;
    [SerializeField] private float stunTime;
    [SerializeField] private float rotationSpeed;
    [SerializeField, Range(0f, 2f)] private float hearingMultiplier = 1f;
    [SerializeField, Range(0.2f, 15f)] private float sightRange = 5f;
    [SerializeField, Range(20f, 90f)] private float sightAngle = 45f;
    [SerializeField, Range(0.5f, 3f)] private float attackRange = 1f;
    [SerializeField] private float attackAntecipationTime = 1;

    [field: SerializeField] public float AttackCooldown { get; private set; }

    public GameEvent Event;


    void Awake()
    {
        
        enemyAnimator = GetComponentInChildren<EnemyAnimator>();
        enemyAnimator.GetAnimator();
        
        AttackState = new EnemyAttackState(this, enemyAnimator);
        ChaseState = new EnemyChaseState(this, enemyAnimator);
        StunState = new EnemyStunState(this, enemyAnimator);
        ParalisedState = new EnemyParalisedState(this, enemyAnimator);
        IdleState = new EnemyIdleState(this, enemyAnimator);
        PatrolState = new EnemyPatrolState(this, enemyAnimator);
        OpenDoorState = new EnemyOpenDoorState(this, enemyAnimator);
        
        agent = GetComponent<NavMeshAgent>();
        PatrolCenterPos = transform.position;

        Paralised = false;

        ChangeState(IdleState);


    }

    private void FixedUpdate() => currentState?.StateFixedUpdate();
    private void Update()
    {
        currentState?.StateUpdate();

        // Update 3D attributes based on the enemy's position and orientation
        FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D
        {
            position = RuntimeUtils.ToFMODVector(transform.position),
            forward = RuntimeUtils.ToFMODVector(transform.forward),
            up = RuntimeUtils.ToFMODVector(transform.up)
        };

        currentAudio.set3DAttributes(attributes);
    }

    public void ApplyStunEffect()
    {
        currentState?.HandleStun();
    }

    public void ApplyEffect()
    {
        Debug.Log("Paralise");
        currentState?.HandleParalise();
    }

    public void RemoveEffect()
    {
        if (Paralised)
        {
            currentState?.HandleChase();
            Paralised = false;
        }
    }

    public virtual void Attack()
    {
        //if (Vector3.Distance(transform.position, playerCharacter.transform.position) <= AttackRange)
            //playerCharacter.GetComponent<PlayerController>().GetDamaged(AttackDamage); Kill Player here
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LightController light))
        {
            light.TurnOnOffLight(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LightController light))
        {
            light.TurnOnOffLight(true);
        }
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