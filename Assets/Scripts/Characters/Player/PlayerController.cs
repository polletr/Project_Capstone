using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using IEnumerator = System.Collections.IEnumerator;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public PlayerSettings Settings;
    public GameEvent Event;

    [field: SerializeField] public Transform CameraHolder { get; private set; }
    [field: SerializeField] public Transform Hand { get; private set; }
    [field: SerializeField] public Transform RechargeRotation { get; private set; }

    public Camera PlayerCam { get; private set; }
    public Vector3 DefaultCameraLocalPosition { get; private set; }

    [field: SerializeField] public bool HasFlashlight { get; set; }
    [SerializeField] private GameObject playerBody;
    public CharacterController characterController { get; private set; }
    public FlashLight flashlight { get; private set; }
    public Interactable interactableObj { get; set; }
    private Transform CheckPoint { get; set; }

    public Coroutine ReloadAnimation { get; set; }

    public float xRotation { get; set; }
    public float yRotation { get; set; }

    public PlayerBaseState currentState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerInteractState InteractState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerCinematicState CinematicState { get; private set; }

    public PlayerRechargeState RechargeState { get; private set; }
    [field: SerializeField] public bool DynamicMov { get; private set; }

    public InputManager inputManager { get; private set; }
    public PlayerAnimator playerAnimator { get; private set; }
    public PlayerInventory playerInventory { get; private set; }
    
    public CameraController camController { get; private set; }

    public EventInstance playerFootsteps { get; private set; }
    public EventInstance playerBreathing { get; private set; }
    public EventInstance playerHeartbeat { get; private set; }

    // private bool _canRegenHealth = true;
    private List<EnemyClass> _enemiesChasing = new();
    private float currentEnemyDistance;

    Coroutine LookAtTargetCoroutine;
    [SerializeField] float lookAtTargetDuration = 2f;
    public Transform DebugTarget;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; //Move this from here later
        Cursor.visible = false; //Move this from here later

        SetSpawn(transform);

        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.GetAnimator();
        inputManager = GetComponent<InputManager>();
        playerInventory = GetComponent<PlayerInventory>();
        camController = GetComponentInChildren<CameraController>();
        camController.SetInputManager(this,inputManager);

        flashlight = GetComponentInChildren<FlashLight>();
        characterController = GetComponent<CharacterController>();

        PlayerCam = CameraHolder.GetComponentInChildren<Camera>();
        DefaultCameraLocalPosition = PlayerCam.transform.localPosition;

        InitializeStates();
        ChangeState(MoveState);
        SetupSoundEvents();
        UpdateFlashlight();
    }

    private void OnEnable()
    {
        Event.OnPickupFlashlight += HandleFlashlightPickUp;
        Event.SetNewSpawn += SetSpawn;
        Event.OnPlayerRespawn += Respawn;
        Event.OnLevelChange += WipeEnemyList;
    }

    private void OnDisable()
    {
        Event.OnPickupFlashlight -= HandleFlashlightPickUp;
        Event.SetNewSpawn -= SetSpawn;
        Event.OnPlayerRespawn -= Respawn;
        Event.OnLevelChange -= WipeEnemyList;
        
        if(playerFootsteps.isValid())
            playerFootsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void Update()
    {
        currentState?.HandleMovement(inputManager.Movement);
        currentState?.HandleLookAround(inputManager.LookAround, inputManager.Device);
        currentState?.StateUpdate();
        if (IsAlive())
            CheckEnemies();

        if (Input.GetKeyDown(KeyCode.L))
            LookAtTarget(DebugTarget);

    }

    private void FixedUpdate() => currentState?.StateFixedUpdate();

    public void GetKilled(EnemyClass enemy, Transform face)
    {
        Event.OnPlayerDeath?.Invoke();
            //Get Killed Logic Here
        DeathState.EnemyFace = face;
        DeathState.EnemyKiller = enemy;

        currentState?.HandleDeath();
    }

    public void GetKilled()
    {
        Event.OnPlayerDeath.Invoke();

        currentState?.HandleDeath();
    }

    public void DynamicMovement(bool on)
    {
        DynamicMov = on;
    }

    private void SetupSoundEvents()
    {
        playerFootsteps =
            AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.PlayerSteps);
        playerBreathing =
            AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.HeavyToLowBreathing);
        playerHeartbeat =
            AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.PlayerHeartbeat);
        RuntimeManager.StudioSystem.setParameterByName("EnemyDistance", 1);
        playerHeartbeat.start();
    }

    private void PlayBreathing()
    {
        playerBreathing.getPlaybackState(out var playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            playerBreathing.start();
        }
    }

    private void UpdateFlashlight()
    {
        switch (HasFlashlight)
        {
            case false when flashlight.gameObject.activeSelf:
                playerBody.gameObject.SetActive(false);
                flashlight.gameObject.SetActive(false);
                break;
            case true when !flashlight.gameObject.activeSelf:
                playerBody.gameObject.SetActive(true);
                flashlight.gameObject.SetActive(true);
                break;
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
        if (_enemiesChasing.Count > 0)
        {
            float nearestDistance = float.MaxValue;
            foreach (ShadowEnemy enemy in _enemiesChasing)
            {
                float distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance < nearestDistance) nearestDistance = distance;

                if (distance > Settings.MaxEnemyDistance)
                {
                    _enemiesChasing.Remove(enemy);
                    break;
                }
            }

            currentEnemyDistance = Mathf.Clamp01(nearestDistance / Settings.MaxEnemyDistance);
        }
        else
        {
            currentEnemyDistance = Mathf.Lerp(currentEnemyDistance, 1f, Time.deltaTime);
        }

        RuntimeManager.StudioSystem.setParameterByName("EnemyDistance", currentEnemyDistance);
    }

    private void WipeEnemyList(LevelData l)
    {
        _enemiesChasing.Clear();
    }

    public void Respawn()
    {
        inputManager.DisablePlayerInput();
        transform.position = CheckPoint.position;
    }

    public void MoveAgainAfterRespawn()
    {
        ChangeState(MoveState);
    }


    public bool IsAlive()
    {
        return currentState != DeathState;
    }

    public void SetSpawn(Transform pos)
    {
        CheckPoint = pos;
    }

    // Smoothly rotates the player to look at a target
    public void LookAtTarget(Transform target)
    {
        // Check if the coroutine is already running
        if (LookAtTargetCoroutine == null)
        {
            Debug.Log("Starting LookAtTarget coroutine...");
            inputManager.DisablePlayerInput();
            LookAtTargetCoroutine = StartCoroutine(SmoothLookAtTarget(target, lookAtTargetDuration));
        }
        else
        {
            Debug.Log("LookAtTarget coroutine is already running.");
        }
    }

    private IEnumerator SmoothLookAtTarget(Transform target, float duration)
    {
        // Calculate target directions
        Vector3 targetDirection = target.position - CameraHolder.position;
        float targetYaw = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg; // Y-axis (horizontal)
        float targetPitch = Mathf.Asin(targetDirection.y / targetDirection.magnitude) * Mathf.Rad2Deg; // X-axis (vertical)

        // Initial rotations
        float initialYaw = yRotation;
        float initialPitch = xRotation;

        // Target rotations
        float targetBodyRotation = targetYaw;
        float targetCameraRotation = -targetPitch;

        // Interpolation
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Interpolate body (y-axis) rotation
            yRotation = Mathf.Lerp(initialYaw, targetBodyRotation, t);
            CameraHolder.localRotation = Quaternion.Euler(0, yRotation, 0);

            // Interpolate camera (x-axis) rotation
            xRotation = Mathf.Lerp(initialPitch, targetCameraRotation, t);
            //xRotation = Mathf.Clamp(xRotation, Settings.ClampAngleUp, Settings.ClampAngleDown);
            PlayerCam.transform.localRotation = Quaternion.Euler(-xRotation, 0, 0);

            yield return null;
        }

        // Finalize the rotation
        yRotation = targetBodyRotation;
        xRotation = targetCameraRotation;
        CameraHolder.localRotation = Quaternion.Euler(0, yRotation, 0);
        PlayerCam.transform.localRotation = Quaternion.Euler(-xRotation, 0, 0);
        LookAtTargetCoroutine = null;
        inputManager.EnablePlayerInput();

    }

    #region Character Actions

    public void HandleInteract()
    {
        currentState?.HandleInteract();
    }

    public void HandleAttack(bool held)
    {
        if (HasFlashlight && flashlight.IsFlashlightOn)
        {
            currentState?.HandleAttack(held);
        }
    }

    public void HandleRecharge()
    {
        if (HasFlashlight && flashlight.BatteryLife < flashlight.MaxBatteryLife * 0.5f)
        {
            currentState?.HandleRecharge();
        }
    }

    //if the flashlight is on, change the flashlight ability to value
    public void HandleChangeAbility(int value, bool isScroll = false)
    {
        if (!flashlight || !flashlight.IsFlashlightOn) return;

        flashlight.HandleChangeAbility(value, isScroll);
    }

    private void HandleFlashlightPickUp()
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
        InteractState = new PlayerInteractState(this);
        MoveState = new PlayerMoveState(this);
        CinematicState = new PlayerCinematicState(this);
        RechargeState = new PlayerRechargeState(this);
    }

    private IEnumerator WaitChangeState(PlayerBaseState newState, float waitTime)
    {
        currentState?.ExitState();
        yield return new WaitForSeconds(waitTime);
        currentState = newState;
        currentState.EnterState();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    #endregion
}