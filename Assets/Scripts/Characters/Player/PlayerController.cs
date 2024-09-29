using UnityEngine;

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

    public InputManager inputManager     {get; private set;}
    public PlayerAnimator playerAnimator {get; private set;}
    public PlayerHealth playerHealth {get; private set;}
    
    private LayerMask groundLayer;

    [SerializeField]
    private GameObject meleeSocketHand;

    public GameObject MeleeSocketHand { get { return meleeSocketHand; } }

    [SerializeField] Transform _camera;
    [SerializeField] Transform _cameraHolder;
    [SerializeField] Transform _hand;

    private void OnEnable()
    {
        Event.OnFlashlightCollect += HandleFlashlightPickUp;
    }

    private void OnDisable()
    {
        Event.OnFlashlightCollect -= HandleFlashlightPickUp;
    }

    void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.GetAnimator();
        inputManager = GetComponent<InputManager>();
        playerHealth = GetComponent<PlayerHealth>();
        
        flashlight = GetComponentInChildren<FlashLight>();
        characterController = GetComponent<CharacterController>();

        Health = Settings.PlayerHealth;
        playerHealth.SetMaxHealth(Health);

        Cursor.lockState = CursorLockMode.Locked;//Move this from here later
        
        groundLayer = LayerMask.GetMask("Ground");
        InitializeStates();
        ChangeState(MoveState);

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

        if(Input.GetKeyDown(KeyCode.O))
        {
            GetDamaged(1f);
            Debug.Log("Health: " + Health);
        }

    }
    private void FixedUpdate() => currentState?.StateFixedUpdate();

    public void GetDamaged(float attackDamage)
    {
        Health -= attackDamage;
        if (Health > 0f)
        {
            currentState?.HandleGetHit();
        }
        else
        {
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
