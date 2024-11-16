using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using IEnumerator = System.Collections.IEnumerator;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerSettings Settings;
    public GameEvent Event;

    [field: SerializeField] public Transform CameraHolder { get; private set; }
    [field: SerializeField] public Transform Hand { get; private set; }

    public Camera PlayerCam { get; private set; }
    public Vector3 DefaultCameraLocalPosition { get; private set; }

    [field: SerializeField] public bool HasFlashlight { get; set; }
    [field: SerializeField] public Transform LookTarget { get; set; }
    [field: SerializeField] public bool CanLookAround { get; set; }
    public CharacterController characterController { get; private set; }
    public FlashLight flashlight { get; private set; }
    public Interactable interactableObj { get; set; }
    public Transform CheckPoint { get; private set; }

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

    public EventInstance playerFootsteps { get; private set; }
    public EventInstance playerBreathing { get; private set; }
    public EventInstance playerHeartbeat { get; private set; }

    // private bool _canRegenHealth = true;
    private List<EnemyClass> _enemiesChasing = new();
    private float currentEnemyDistance;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; //Move this from here later
        Cursor.visible = false; //Move this from here later

        SetSpawn(transform);

        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.GetAnimator();
        inputManager = GetComponent<InputManager>();
        playerInventory = GetComponent<PlayerInventory>();

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
    }

    private void Update()
    {
        currentState?.HandleMovement(inputManager.Movement);
        currentState?.HandleLookAround(inputManager.LookAround, inputManager.Device);
        currentState?.StateUpdate();
        if (IsAlive())
            CheckEnemies();
    }

    private void FixedUpdate() => currentState?.StateFixedUpdate();

    public void GetKilled(EnemyClass enemy, Transform face)
    {
        //Get Killed Logic Here
        DeathState.EnemyFace = face;
        DeathState.EnemyKiller = enemy;

        currentState?.HandleDeath();
    }

    public void GetKilled()
    {
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
                flashlight.gameObject.SetActive(false);

                break;
            case true when !flashlight.gameObject.activeSelf:
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
            Debug.Log("Recharging Flashlight");
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