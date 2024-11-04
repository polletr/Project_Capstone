using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class ShadowEnemy : EnemyClass, IStunable
{
    public Vector3 PatrolCenterPos { get; set; }
    public PlayerController playerCharacter { get; set; }
    public bool Paralised { get; set; }
    public Door TargetDoor { get; set; }
    public EventInstance currentAudio{ get; set; }

    [SerializeField] private List<Transform> patrolTransforms = new List<Transform>();
    [SerializeField] private Transform eyes;

    public UnityEvent OnStunEnemy;


    public float MaxIdleTime
    {
        get { return maxIdleTime; }
    }

    public float MinIdleTime
    {
        get { return minIdleTime; }
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
    public Collider EnemyCollider { get; private set; }



    public EnemyBaseState currentState;
    public EnemyAnimator enemyAnimator { get; set; }

    [HideInInspector] public NavMeshAgent agent;

    [SerializeField] private float patrolRange;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float paralisedSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField, Range(0f, 2f)] private float hearingMultiplier = 1f;
    [SerializeField, Range(0.2f, 100f)] private float sightRange = 5f;
    [SerializeField, Range(0.5f, 3f)] private float attackRange = 1f;
    [SerializeField] private float attackAntecipationTime = 1;
    [field: SerializeField] public float TeleportCooldown { get; private set; }
    [field: SerializeField] public float MaxChaseTime { get; private set; }
    [field: SerializeField] public float StunTime { get; private set; }
    [field: SerializeField] public Material BodyMaterial { get; private set; }
    [field: SerializeField] public Material EyeMaterial { get; private set; }
    [field: SerializeField] public ParticleSystem SmokeParticle { get; private set; }
    [field: SerializeField] public float DoorAttackCooldown { get; private set; }
    [field: SerializeField] public float DoorAttackDamage { get; private set; }
    [SerializeField] private float changeTranspDuration;
    public bool EnemyTeleporting { get; private set; }

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

        EnemyCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        PatrolCenterPos = transform.position;

        BodyMaterial.SetFloat("_Transparency", 0.9f);
        EyeMaterial.SetFloat("_Transparency", 0.9f);


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
        Debug.Log("Enemy Received Stun");
        currentState?.HandleStun();
    }

    public void ApplyEffect()
    {
        currentState?.HandleParalise();
    }

    public void RemoveEffect()
    {
        if (Paralised)
            currentState?.HandleChase();
    }

    public virtual void Attack()
    {
        playerCharacter.GetKilled(this, eyes);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LightController light))
        {
            if (!light.GuidingLight)
                light.TurnOnOffLight(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LightController light))
        {
            if (!light.GuidingLight)
                light.TurnOnOffLight(true);
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

    public IEnumerator EnemyTransparency(float targetTransp)
    {
        // Get current transparency (alpha) values from the body and eye materials
        float bodyCurrentTransparency = BodyMaterial.GetFloat("_Transparency");
        float eyeCurrentTransparency = EyeMaterial.GetFloat("_Transparency");

        // Store the starting values for interpolation
        float startBodyTransparency = bodyCurrentTransparency;
        float startEyeTransparency = eyeCurrentTransparency;

        float elapsedTime = 0f;

        // Gradually change transparency over the duration
        while (elapsedTime < changeTranspDuration)
        {
            // Calculate the new transparency using Lerp
            float newBodyTransparency = Mathf.Lerp(startBodyTransparency, targetTransp, elapsedTime / changeTranspDuration);
            float newEyeTransparency = Mathf.Lerp(startEyeTransparency, targetTransp, elapsedTime / changeTranspDuration);

            // Set the new transparency values back to the materials
            BodyMaterial.SetFloat("_Transparency", newBodyTransparency);
            EyeMaterial.SetFloat("_Transparency", newEyeTransparency);

            // Increase the elapsed time
            elapsedTime += Time.deltaTime;

            // Yield until the next frame
            yield return null;
        }

        // Ensure the transparency is set to the target value at the end
        BodyMaterial.SetFloat("_Transparency", targetTransp);
        EyeMaterial.SetFloat("_Transparency", targetTransp);
    }

    public IEnumerator TeleportEnemyWithDelay(Vector3 desiredTeleportPosition)
    {
        EnemyTeleporting = true;
        // Play the smoke particle effect
        //SmokeParticle.Play();
        // Start fading the enemy out to 0 transparency
        yield return StartCoroutine(EnemyTransparency(0f));

/*        // Wait for the specified time before teleporting
        yield return new WaitForSeconds(changeTranspDuration);
*/
        // Teleport the enemy to the desired position
        transform.position = desiredTeleportPosition;
        // Start fading the enemy back to the desired transparency (e.g., 0.9f)
        yield return StartCoroutine(EnemyTransparency(0.9f));
        EnemyTeleporting = false;

    }

    public void OnDisable()
    {
        if(playerCharacter != null)
        {
            playerCharacter.RemoveEnemyFromChaseList(this);
        }
        currentAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

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