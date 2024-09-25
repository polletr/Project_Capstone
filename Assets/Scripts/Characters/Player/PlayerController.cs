using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{

    public PlayerSettings Settings { get { return settings; } }

    public GameEvent Event;
    public Inventory inventory;

    public Transform Camera { get { return _camera; } }
    public Transform CameraHolder { get { return _cameraHolder; } }
    public Transform Hand { get { return _hand; } }

    public float Health { get; private set; }
    public float InteractionRange { get { return interactionRange; } }


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

    private InputManager inputManager;
    private PlayerAnimator playerAnimator;
    
    private LayerMask groundLayer;

    [SerializeField]
    private GameObject meleeSocketHand;

    public GameObject MeleeSocketHand { get { return meleeSocketHand; } }

    [SerializeField] Transform _camera;
    [SerializeField] Transform _cameraHolder;
    [SerializeField] Transform _hand;

    

    void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.GetAnimator();
        Cursor.lockState = CursorLockMode.Locked;//Move this from here later
        groundLayer = LayerMask.GetMask("Ground");
        inputManager = GetComponent<InputManager>();
        flashlight = GetComponentInChildren<FlashLight>();
        characterController = GetComponent<CharacterController>();
        Health = Settings.PlayerHealth;
        InitializeStates();
        ChangeState(MoveState);
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
