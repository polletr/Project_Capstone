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

    public void EnablePlayerInput()
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

    public void DisablePlayerInput()
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
    }

    private void OnPointerMove(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        Device = context.control.device;
        LookAround = input;
    }

    private void TogglePause()
    {
        pauseMenu.OnTogglePauseMenu();
        
        if (pauseMenu.IsPaused)
            DisablePlayerInput();
        else
            EnablePlayerInput();
    }
}