using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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

    private void OnEnable() => EnableInput();

    private void OnDisable() => DisableInput();

    private void EnableInput()
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

        action.Player.ChangeItem.performed += (val) => HandleScrollAbility(val.ReadValue<Vector2>());

        action.Player.ChangeBattery.performed += (val) => player.HandleChangeBattery();
        if (pauseMenu != null)
            action.Menu.Pause.performed += (val) => pauseMenu.OnTogglePauseMenu();

        action.Enable();
    }

    private void DisableInput()
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

        action.Player.ChangeItem.performed -= (val) => HandleScrollAbility(val.ReadValue<Vector2>());

        action.Player.ChangeBattery.performed -= (val) => player.HandleChangeBattery();
        if (pauseMenu != null)
        action.Menu.Pause.performed -= (val) => pauseMenu.OnTogglePauseMenu();

        action.Disable();
    }

    private void HandleScrollAbility(Vector2 scrollValue)
    {
        if (scrollValue.y > 0 || scrollValue.x > 0)
            player.currentState?.HandleChangeAbility(1);
        else if (scrollValue.y < 0 || scrollValue.x < 0)
            player.currentState?.HandleChangeAbility(-1);
    }

    private void OnPointerMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Device = context.control.device;
        LookAround = input;
    }
}