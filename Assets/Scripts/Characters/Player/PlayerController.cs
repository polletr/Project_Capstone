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

    public CharacterController characterController { get; set; }
    public Animator animator { get; set; }
    public FlashLight flashlight { get; set; }

    public float xRotation { get; set; }
    public float yRotation { get; set; }



    [HideInInspector]
    public PlayerBaseState currentState;

    [SerializeField]
    private PlayerSettings settings;

    private InputManager inputManager;
    private LayerMask groundLayer;

    [SerializeField]
    private GameObject meleeSocketHand;

    public GameObject MeleeSocketHand { get { return meleeSocketHand; } }

    [SerializeField] Transform _camera;
    [SerializeField] Transform _cameraHolder;
    [SerializeField] Transform _hand;


    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;//Move this from here later
        groundLayer = LayerMask.GetMask("Ground");
        inputManager = GetComponent<InputManager>();
        flashlight = GetComponentInChildren<FlashLight>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        Health = Settings.PlayerHealth;
        ChangeState(new PlayerMoveState());
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



    #endregion


    #region ChangeState
    public void ChangeState(PlayerBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.player = this;
        currentState.EnterState();
    }

    /*    private IEnumerator WaitFixedFrame(PlayerBaseState newState)
        {

            yield return new WaitForFixedUpdate();

        }
    */
    #endregion


}
