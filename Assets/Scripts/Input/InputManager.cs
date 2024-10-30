using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerInput action;
    private PlayerController player;
    private CameraController cameraController;

    public Vector2 Movement { get; private set; }

    public Vector2 LookAround { get; private set; }

    public InputDevice Device { get; private set; }

    [SerializeField] private PauseMenu pauseMenu;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerController>();
        cameraController = GetComponentInChildren<CameraController>();
        action = new PlayerInput();
    }

    private void OnEnable()
    {
        EnablePlayerInput();
        if (pauseMenu != null)
        {
            action.Menu.Pause.performed += (val) => TogglePause();
            pauseMenu.OnResume.AddListener(TogglePause);
        }

        action.Enable();
    }

    private void OnDisable()
    {
        DisablePlayerInput();
        if (pauseMenu != null)
        {
            action.Menu.Pause.performed -= (val) => TogglePause();
            pauseMenu.OnResume.RemoveListener(TogglePause);
        }

        action.Disable();
    }

    #region Player Input

    public void EnablePlayerInput()
    {
        action.Player.Move.performed += OnMove;

        action.Player.PointerMove.performed += OnPointerMove;

        action.Player.Attack.performed += (_) => player.HandleAttack(true);
        action.Player.Attack.canceled += (_) => player.HandleAttack(false);

        action.Player.Crouch.performed += (_) => player.currentState?.HandleCrouch(true);
        action.Player.Crouch.canceled += (_) => player.currentState?.HandleCrouch(false);

        action.Player.Run.canceled += (_) => player.currentState?.HandleRun(true);
        action.Player.Run.performed += (_) => player.currentState?.HandleRun(true);

        action.Player.Interact.performed += OnInteract;

        action.Player.RechargeFlashlight.performed += OnRechargeFlashlight;

        action.Player.Flashlight.performed += OnFlashlightPower;

        action.Player.ChangeAbility.performed += OnChangeAbility;
        
        action.Player.ChangeAbilityMouse.performed += HandleScrollAbility;
    }

    public void DisablePlayerInput()
    {
        action.Player.Move.performed -= OnMove;

        action.Player.PointerMove.performed -= OnPointerMove;

        action.Player.Attack.canceled  -= (_) => player.HandleAttack(false);
        action.Player.Attack.performed -= (_) => player.HandleAttack(true);

        
        action.Player.Crouch.performed -= (_) => player.currentState?.HandleCrouch(true);
        action.Player.Crouch.canceled -= (_) => player.currentState?.HandleCrouch(false);

        action.Player.Run.performed -= (_) => player.currentState?.HandleRun(true);
        action.Player.Run.canceled -= (_) => player.currentState?.HandleRun(true);

        action.Player.Interact.performed -= OnInteract;

        action.Player.RechargeFlashlight.performed -= OnRechargeFlashlight;

        action.Player.Flashlight.performed -= OnFlashlightPower;

        action.Player.ChangeAbility.performed -= OnChangeAbility;
        
        action.Player.ChangeAbilityMouse.performed -= HandleScrollAbility;
    }
    #endregion

    /// <summary>
    /// All the input for the player actions
    /// </summary>

    #region Player Game Input

    private void OnMove(InputAction.CallbackContext context) => Movement = context.ReadValue<Vector2>();

    private void OnPointerMove(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        Device = context.control.device;
        LookAround = input;
    }

    private void OnInteract(InputAction.CallbackContext context) => player.HandleInteract();

    private void OnRechargeFlashlight(InputAction.CallbackContext context) => player.HandleRecharge();

    private void OnFlashlightPower(InputAction.CallbackContext context) => player.currentState?.HandleFlashlightPower();

    private void OnChangeAbility(InputAction.CallbackContext context) => player.HandleChangeAbility((int)context.ReadValue<float>());
    
    private void HandleScrollAbility(InputAction.CallbackContext context)
    {
        var scrollValue = context.ReadValue<Vector2>();
        if (scrollValue.y > 0 || scrollValue.x > 0)
        {
            player.HandleChangeAbility(1,true);
        }
        else if (scrollValue.y < 0 || scrollValue.x < 0)
        {
            player.HandleChangeAbility(-1,true);
        }
    }

    #endregion

    /// <summary>
    /// All the input for the player Menu actions
    /// </summary>

    #region Player Menu Input

    private void TogglePause()
    {
        pauseMenu.OnTogglePauseMenu();

        if (pauseMenu.IsPaused)
            DisablePlayerInput();
        else
            EnablePlayerInput();
    }

    #endregion
}