using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NurseEnemy : EnemyClass
{
    public PlayerController playerCharacter { get; set; }
    public EventInstance currentAudio{ get; set; }

    [SerializeField] private List<Transform> patrolTransforms = new List<Transform>();
    [SerializeField] private Transform eyes;

    [field: SerializeField] public float RotationSpeed { get; private set; }

    public List<Transform> PatrolTransforms
    {
        get { return patrolTransforms; }
        set { patrolTransforms = value; }
    }
    public float AttackRange
    {
        get { return attackRange; }
    }

    public NurseAttackState AttackState { get; private set; }
    public NurseChaseState ChaseState { get; private set; }
    public NurseRetreatState RetreatState { get; private set; }
    public NurseIdleState IdleState { get; private set; }
    public Collider EnemyCollider { get; private set; }

    public NurseBaseState currentState;
    public EnemyAnimator enemyAnimator { get; set; }

    [HideInInspector] public NavMeshAgent agent;

    [SerializeField, Range(0.5f, 3f)] private float attackRange = 1f;
    [SerializeField] private float attackAntecipationTime = 1;
    public GameEvent Event;


    void Awake()
    {
        
        enemyAnimator = GetComponentInChildren<EnemyAnimator>();
        enemyAnimator.GetAnimator();
        
        AttackState = new NurseAttackState(this, enemyAnimator);
        ChaseState = new NurseChaseState(this, enemyAnimator);
        RetreatState = new NurseRetreatState(this, enemyAnimator);
        IdleState = new NurseIdleState(this, enemyAnimator);

        EnemyCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();

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

    public virtual void Attack()
    {
        playerCharacter.GetKilled(this, eyes);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RevealableLight"))
        {
            currentState?.HandleRetreat();
        }
    }

    public void DisableEnemy()
    {
        if (playerCharacter != null)
        {
            playerCharacter.RemoveEnemyFromChaseList(this);
        }
        currentAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        this.gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        if(playerCharacter != null)
        {
            playerCharacter.RemoveEnemyFromChaseList(this);
        }
        currentAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }

    #region ChangeState

    public void ChangeState(NurseBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.enemy = this;
        currentState.EnterState();
    }

    #endregion
}