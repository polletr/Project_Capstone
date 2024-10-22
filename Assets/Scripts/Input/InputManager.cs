using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput action;
    private PlayerController player;
    private CameraController cameraController;

    public Vector2 Movement { get; private set; }

    public Vector2 LookAround { get; private set; }

    public InputDevice Device { get; private set; }

    [SerializeField] private PauseMenu pauseMenu;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        cameraController = GetComponentInChildren<CameraController>();
        action = new PlayerInput();
    }

    private void OnEnable()
    {
        EnablePlayerInput();
        if (pauseMenu != null)
            action.Menu.Pause.performed += TogglePause;

        action.Enable();
    }

    private void OnDisable()
    {
        DisablePlayerInput();
        if (pauseMenu != null)
            action.Menu.Pause.performed -= TogglePause;
        
        action.Disable();
    }

    private void EnablePlayerInput()
    {
        action.Player.Move.performed += (val) => Movement = val.ReadValue<Vector2>();

        action.Player.PointerMove.performed += OnPointerMove;

        action.Player.Attack.performed += (val) => player.currentState?.HandleAttack(true);
        action.Player.Attack.canceled += (val) => player.currentState?.HandleAttack(false);

        action.Player.Crouch.performed += (val) => player.currentState?.HandleCrouch(true);
        action.Player.Crouch.canceled += (val) => player.currentState?.HandleCrouch(false);

        action.Player.Run.performed += (val) => player.currentState?.HandleRun(true);
        action.Player.Run.canceled += (val) => player.currentState?.HandleRun(false);

        action.Player.Interact.performed += (val) => player.currentState?.HandleInteract();

        action.Player.Flashlight.performed += (val) => player.currentState?.HandleFlashlightPower();

        action.Player.ChangeBattery.performed += (val) => player.HandleChangeBattery();
    }

    private void DisablePlayerInput()
    {
        action.Player.Move.performed -= (val) => Movement = val.ReadValue<Vector2>();

        action.Player.PointerMove.performed -= OnPointerMove;

        action.Player.Attack.performed -= (val) => player.currentState?.HandleAttack(true);
        action.Player.Attack.canceled -= (val) => player.currentState?.HandleAttack(false);

        action.Player.Crouch.performed -= (val) => player.currentState?.HandleCrouch(true);
        action.Player.Crouch.canceled -= (val) => player.currentState?.HandleCrouch(false);

        action.Player.Run.performed -= (val) => player.currentState?.HandleRun(true);
        action.Player.Run.canceled -= (val) => player.currentState?.HandleRun(false);

        action.Player.Interact.performed -= (val) => player.currentState?.HandleInteract();

        action.Player.Flashlight.performed -= (val) => player.currentState?.HandleFlashlightPower();

        action.Player.ChangeBattery.performed -= (val) => player.HandleChangeBattery();
        if (pauseMenu != null)
            action.Menu.Pause.performed -= (val) => pauseMenu.OnTogglePauseMenu();
    }

    private void OnPointerMove(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        Device = context.control.device;
        LookAround = input;
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        pauseMenu.OnTogglePauseMenu();
        
        if (pauseMenu.IsPaused)
            DisablePlayerInput();
        else
            EnablePlayerInput();
    }
}