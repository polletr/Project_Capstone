using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMOD;

public class PlayerController : MonoBehaviour, IDamageable
{
    public PlayerSettings Settings { get { return settings; } }

    public GameEvent Event;

    public Transform Camera { get { return _camera; } }
    public Transform CameraHolder { get { return _cameraHolder; } }
    public Transform Hand { get { return _hand; } }

    [SerializeField] private float health;
    public float Health
    {
        get => health;
        private set
        {
            health = value;
            playerHealth.SetHealth(value); 
        }
    }

    private Transform CheckPoint;

    private bool canRegenHealth = true;

    public float InteractionRange { get { return interactionRange; } }

    [field: SerializeField] public bool HasFlashlight { get; set; }

    public CharacterController characterController { get; set; }
    public FlashLight flashlight { get; set; }
    public IInteractable interactableObj { get; set; }

    public float xRotation { get; set; }
    public float yRotation { get; set; }

    [SerializeField] private float interactionRange;

     public PlayerBaseState currentState  { get; private set; } 
     public PlayerAttackState AttackState { get; private set; }       
     public PlayerDeathState DeathState { get; private set; }
     public PlayerGetHitState GetHitState { get; private set; } 
     public PlayerInteractState InteractState  { get; private set; }
     public PlayerMoveState MoveState { get; private set; }
    
    

    [SerializeField]
    private PlayerSettings settings;

    private Coroutine healthRegenCoroutine;

    public InputManager inputManager     {get; private set;}
    public PlayerAnimator playerAnimator {get; private set;}
    public PlayerHealth playerHealth {get; private set;}
    
    private LayerMask groundLayer;

    private float minEnemyDistance;

    [SerializeField]
    private GameObject meleeSocketHand;
    public GameObject MeleeSocketHand { get { return meleeSocketHand; } }

    [SerializeField] Transform _camera;
    [SerializeField] Transform _cameraHolder;
    [SerializeField] Transform _hand;
    public Transform DeathParentObj;

    public Camera PlayerCam { get; private set; }

    private List<EnemyClass> enemiesChasing = new();
    public EventInstance playerFootsteps { get; private set; }
    public EventInstance playerBreathing { get; private set; }
    public EventInstance playerHeartbeat { get; private set; }


    private void OnEnable()
    {
        Event.OnFlashlightCollect += HandleFlashlightPickUp;
        Event.SetNewSpawn += SetSpawn;
    }

    private void OnDisable()
    {
        Event.OnFlashlightCollect -= HandleFlashlightPickUp;
        Event.SetNewSpawn -= SetSpawn;
    }


    void Awake()
    {
        SetSpawn(transform);
        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.GetAnimator();
        inputManager = GetComponent<InputManager>();
        playerHealth = GetComponent<PlayerHealth>();
        
        PlayerCam = Camera.GetComponentInChildren<Camera>();

        flashlight = GetComponentInChildren<FlashLight>();
        characterController = GetComponent<CharacterController>();

        Health = Settings.PlayerHealth;
        playerHealth.SetMaxHealth(Health);

        Cursor.lockState = CursorLockMode.Locked;//Move this from here later
        
        groundLayer = LayerMask.GetMask("Ground");
        InitializeStates();
        ChangeState(MoveState);
        SetupSoundEvents();

        if (!HasFlashlight && flashlight.gameObject.activeSelf)
        {
            flashlight.gameObject.SetActive(false);
        }
        else if (HasFlashlight && !flashlight.gameObject.activeSelf)
        {
            flashlight.gameObject.SetActive(true);
        }

    }

    private void InitializeStates()
    {
        AttackState = new PlayerAttackState(playerAnimator,this,inputManager);
        DeathState = new PlayerDeathState(playerAnimator,this,inputManager);
        GetHitState = new PlayerGetHitState(playerAnimator,this,inputManager);
        InteractState = new PlayerInteractState(playerAnimator,this,inputManager);    
        MoveState = new PlayerMoveState(playerAnimator,this,inputManager);
    }

    private void Update()
    {
        currentState?.HandleMovement(inputManager.Movement);
        currentState?.HandleLookAround(inputManager.LookAround, inputManager.Device);
        currentState?.StateUpdate();

        RuntimeManager.StudioSystem.setParameterByName("Health", Health / Settings.PlayerHealth);
        CheckEnemies();
        if (Input.GetKeyDown(KeyCode.O))
        {
            GetDamaged(1f);
        }

        RegenerateHealth(); 

    }
    private void FixedUpdate() => currentState?.StateFixedUpdate();

    public void GetDamaged(float attackDamage)
    {
        Health -= attackDamage;

        PlayBreathing();
        if (Health > 0f)
        {
            currentState?.HandleGetHit();
            if (healthRegenCoroutine != null)
                StopCoroutine(healthRegenCoroutine);
            healthRegenCoroutine = StartCoroutine(DelayHealthRegen());
        }
        else
        {
            if(currentState!= DeathState)
            currentState?.HandleDeath();
        }

    }

    public void HandleFlashlightPickUp(bool check)
    {
        HasFlashlight = check;
        if (!HasFlashlight && flashlight.gameObject.activeSelf)
        {
            flashlight.gameObject.SetActive(false);
        }
        else if (HasFlashlight && !flashlight.gameObject.activeSelf)
        {
            flashlight.gameObject.SetActive(true);
        }
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    private void RegenerateHealth()
    {
        if (IsAlive() && canRegenHealth && Health < Settings.PlayerHealth && enemiesChasing.Count <= 0)//&& flashlight.) // check is player is not dead and flashlight is on / is player in light ( not in dark area)
        {
            Health += Settings.HealthRegenRate * Time.deltaTime;
            Health = Mathf.Clamp(Health, 0, Settings.PlayerHealth);
        }
    }

    private IEnumerator DelayHealthRegen()
    {
        //We also need to check if we are out of danger!
        canRegenHealth = false;
        yield return new WaitForSecondsRealtime(Settings.HealthRegenDelay);
        canRegenHealth = true;
        healthRegenCoroutine = null;
    }


    void SetupSoundEvents()
    {
        playerFootsteps = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.PlayerSteps);
        playerBreathing = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.HeavyToLowBreathing);
        playerHeartbeat = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.PlayerHeartbeat);
        RuntimeManager.StudioSystem.setParameterByName("EnemyDistance", 1);
        playerHeartbeat.start();

    }

    void PlayBreathing()
    {
        PLAYBACK_STATE playbackState;
        playerBreathing.getPlaybackState(out playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            playerBreathing.start();
        }
    }

    public void AddEnemyToChaseList(EnemyClass enemy)
    {
        enemiesChasing.Add(enemy);
    }

    public void RemoveEnemyFromChaseList(EnemyClass enemy)
    {
        if (enemiesChasing.Contains(enemy))
        {
            enemiesChasing.Remove(enemy);
        }
    }

    private void CheckEnemies()
    {
        if (enemiesChasing.Count == 0)
            return;

        foreach(EnemyClass enemy in enemiesChasing)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) < minEnemyDistance)
            {
                minEnemyDistance = Vector3.Distance(enemy.transform.position, transform.position);
            }
            
            if (Vector3.Distance(enemy.transform.position, transform.position) > Settings.MaxEnemyDistance)
            {
                enemiesChasing.Remove(enemy);
                break;
            }
        }

        if (enemiesChasing.Count > 0)
            RuntimeManager.StudioSystem.setParameterByName("EnemyDistance", minEnemyDistance / Settings.MaxEnemyDistance);
        else
            RuntimeManager.StudioSystem.setParameterByName("EnemyDistance", 1);

    }

    public void Respawn()
    {
        playerAnimator.transform.position = CheckPoint.position;
        Health = Settings.PlayerHealth;
        ChangeState(MoveState);
    }

    private void SetSpawn(Transform pos)
    {
        CheckPoint = pos;
    }


    #region Character Actions

    public void HandleInteract()
    {
        currentState?.HandleInteract();
    }

    public void CancelInteract()
    {

    }

    public void HandleChangeBattery()
    {
        flashlight.RemoveOldBattery();
        Event.OnAskForBattery?.Invoke();
    }



    #endregion


    #region ChangeState
    public void ChangeState(PlayerBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    /*    private IEnumerator WaitFixedFrame(PlayerBaseState newState)
        {

            yield return new WaitForFixedUpdate();

        }
    */
    #endregion


}
