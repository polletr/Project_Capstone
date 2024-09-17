using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInput _action;
    PlayerController _player;

    Vector2 _movement;
    public Vector2 Movement
    {
        get
        {
            return _movement;
        }
        private set
        {
            _movement = value;
        }
    }
    void Awake()
    {
        _player = GetComponent<PlayerController>();
        _action = new PlayerInput();
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    public void EnableInput()
    {
        _action.Player.Move.performed += (val) => Movement = val.ReadValue<Vector2>();
        _action.Player.PointerMove.performed += OnPointerMove;
        _action.Player.Attack.performed += (val) => _player.currentState?.HandleAttack();
        _action.Player.Interact.performed += (val) => _player.currentState?.HandleInteract();
        _action.Player.DropItem.performed += (val) => _player.HandleDropItem();
        _action.Player.DropItem.canceled += (val) => _player.currentState?.CancelDropItem();
        _action.Player.ChangeItem.performed += (val) => HandleScrollWeapon(val.ReadValue<Vector2>());
        _action.Player.Run.performed += (val) => _player.currentState?.HandleRun(true);
        _action.Player.Run.canceled += (val) => _player.currentState?.HandleRun(false);
        _action.Player.Crouch.performed += (val) => _player.currentState?.HandleCrouch(true);
        _action.Player.Crouch.canceled += (val) => _player.currentState?.HandleCrouch(false);


        _action.Enable();

    }
    public void DisableInput()
    {
        _action.Player.Move.performed -= (val) => Movement = val.ReadValue<Vector2>();
        _action.Player.PointerMove.performed -= OnPointerMove;
        _action.Player.Attack.performed -= (val) => _player.currentState?.HandleAttack();
        _action.Player.Interact.performed -= (val) => _player.currentState?.HandleInteract();
        _action.Player.DropItem.performed -= (val) => _player.HandleDropItem();
        _action.Player.DropItem.canceled -= (val) => _player.currentState?.CancelDropItem();
        _action.Player.ChangeItem.performed -= (val) => HandleScrollWeapon(val.ReadValue<Vector2>());
        _action.Player.Run.performed -= (val) => _player.currentState?.HandleRun(true);
        _action.Player.Run.canceled -= (val) => _player.currentState?.HandleRun(false);
        _action.Player.Crouch.performed -= (val) => _player.currentState?.HandleCrouch(true);
        _action.Player.Crouch.canceled -= (val) => _player.currentState?.HandleCrouch(false);


        _action.Disable();

    }

    private void HandleScrollWeapon(Vector2 scrollValue)
    {
        if (scrollValue.y > 0 || scrollValue.x > 0)
        {
            _player.HandleChangeItem(1);
        }
        else if (scrollValue.y < 0 || scrollValue.x < 0)
        {
            _player.HandleChangeItem(-1);
        }
    }

    private void OnPointerMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        InputDevice device = context.control.device;

        if (device is Pointer) // This includes Mouse, Pen, Touch, etc.
        {
            _player.HandleMouseInput(input);
        }
        else if (device is Gamepad) // This includes any kind of game controller
        {
            _player.HandleGamepadInput(input);
        }
    }

}
