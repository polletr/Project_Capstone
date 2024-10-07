using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    public PlayerSettings Settings;
    public GameEvent Event;

    [field: SerializeField] public Transform CameraHolder { get; private set; }
    [field: SerializeField] public Transform DeathCamPos { get; private set; }
    public Camera PlayerCam { get; private set; }
    public Vector3 DefaultCameraLocalPosition { get; set; }

    private float _currentHealth;
    public float Health
    {
        get => _currentHealth;
        private set
        {
            _currentHealth = value;
            playerHealth.SetHealth(value);
        }
    }

    [field: SerializeField] public bool HasFlashlight { get; set; }
    public CharacterController characterController { get; set; }
    public FlashLight flashlight { get; set; }
    public IInteractable interactableObj { get; set; }

    public float xRotation { get; set; }
    public float yRotation { get; set; }

    public PlayerBaseState currentState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerGetHitState GetHitState { get; private set; }
    public PlayerInteractState InteractState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }

    public InputManager inputManager { get; private set; }
    public PlayerAnimator playerAnimator { get; private set; }
    public PlayerHealth playerHealth { get; private set; }

    public EventInstance playerFootsteps { get; private set; }
    public EventInstance playerBreathing { get; private set; }
    public EventInstance playerHeartbeat { get; private set; }

    private float _minEnemyDistance;
    private bool _canRegenHealth = true;
    private Transform _checkPoint;
    private Coroutine _healthRegenCoroutine;
    private List<EnemyClass> _enemiesChasing = new();


    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;//Move this from here later
        Cursor.visible = false;//Move this from here later

        SetSpawn(transform);

        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.GetAnimator();
        inputManager = GetComponent<InputManager>();
        playerHealth = GetComponent<PlayerHealth>();

        flashlight = GetComponentInChildren<FlashLight>();
        characterController = GetComponent<CharacterController>();

        PlayerCam = CameraHolder.GetComponentInChildren<Camera>();
        DefaultCameraLocalPosition = PlayerCam.transform.localPosition;

        Health = Settings.PlayerHealth;

        InitializeStates();
        ChangeState(MoveState);
        SetupSoundEvents();
        UpdateFlashlight();
    }

    private void OnEnable()
    {
        Event.OnPickupFlashlight += HandleFlashlightPickUp;
        Event.SetNewSpawn += SetSpawn;
    }

    private void OnDisable()
    {
        Event.OnPickupFlashlight -= HandleFlashlightPickUp;
        Event.SetNewSpawn -= SetSpawn;
    }

    private void Update()
    {
        currentState?.HandleMovement(inputManager.Movement);
        currentState?.HandleLookAround(inputManager.LookAround, inputManager.Device);
        currentState?.StateUpdate();

        RuntimeManager.StudioSystem.setParameterByName("Health", Health / Settings.PlayerHealth);

        CheckEnemies();
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
            if (_healthRegenCoroutine != null)
                StopCoroutine(_healthRegenCoroutine);
            _healthRegenCoroutine = StartCoroutine(DelayHealthRegen());
        }
        else
        {
            if (currentState != DeathState)
                currentState?.HandleDeath();
        }

    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    private void RegenerateHealth()
    {
        if (IsAlive() && _canRegenHealth && Health < Settings.PlayerHealth && _enemiesChasing.Count <= 0)//&& flashlight.) // check is player is not dead and flashlight is on / is player in light ( not in dark area)
        {
            Health += Settings.HealthRegenRate * Time.deltaTime;
            Health = Mathf.Clamp(Health, 0, Settings.PlayerHealth);
        }
    }

    private IEnumerator DelayHealthRegen()
    {
        //We also need to check if we are out of danger!
        _canRegenHealth = false;
        yield return new WaitForSecondsRealtime(Settings.HealthRegenDelay);
        _canRegenHealth = true;
        _healthRegenCoroutine = null;
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

    private void UpdateFlashlight()
    {
        if (!HasFlashlight && flashlight.gameObject.activeSelf)
        {
            flashlight.gameObject.SetActive(false);
        }
        else if (HasFlashlight && !flashlight.gameObject.activeSelf)
        {
            flashlight.gameObject.SetActive(true);
        }

    }

    public void AddEnemyToChaseList(EnemyClass enemy)
    {
        _enemiesChasing.Add(enemy);
    }

    public void RemoveEnemyFromChaseList(EnemyClass enemy)
    {
        if (_enemiesChasing.Contains(enemy))
        {
            _enemiesChasing.Remove(enemy);
        }
    }

    private void CheckEnemies()
    {
        if (_enemiesChasing.Count == 0)
            return;

        foreach (EnemyClass enemy in _enemiesChasing)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) < _minEnemyDistance)
            {
                _minEnemyDistance = Vector3.Distance(enemy.transform.position, transform.position);
            }

            if (Vector3.Distance(enemy.transform.position, transform.position) > Settings.MaxEnemyDistance)
            {
                _enemiesChasing.Remove(enemy);
                break;
            }
        }

        if (_enemiesChasing.Count > 0)
            RuntimeManager.StudioSystem.setParameterByName("EnemyDistance", _minEnemyDistance / Settings.MaxEnemyDistance);
        else
            RuntimeManager.StudioSystem.setParameterByName("EnemyDistance", 1);

    }

    public void Respawn()
    {
        playerAnimator.transform.position = _checkPoint.position;
        Health = Settings.PlayerHealth;
        ChangeState(MoveState);
    }

    private void SetSpawn(Transform pos)
    {
        _checkPoint = pos;
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

    public void HandleFlashlightPickUp()
    {
        HasFlashlight = true;
        UpdateFlashlight();
    }

    #endregion


    #region ChangeState

    private void InitializeStates()
    {
        AttackState = new PlayerAttackState(this);
        DeathState = new PlayerDeathState(this);
        GetHitState = new PlayerGetHitState(this);
        InteractState = new PlayerInteractState(this);
        MoveState = new PlayerMoveState(this);
    }

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
