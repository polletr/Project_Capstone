using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;

public class InputManager : MonoBehaviour
{
    PlayerInput _action;
    PlayerController _player;
    CameraController _cameraController;

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

    Vector2 _lookAround;
    public Vector2 LookAround
    {
        get
        {
            return _lookAround;
        }
        private set
        {
            _lookAround = value;
        }
    }

    InputDevice _device;
    public InputDevice Device
    {
        get
        {
            return _device;
        }
        private set
        {
            _device = value;
        }
    }


    void Awake()
    {
        _player = GetComponent<PlayerController>();
        _cameraController = GetComponentInChildren<CameraController>();
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
        _action.Player.Move.performed += (val) => _movement = val.ReadValue<Vector2>();
        _action.Player.PointerMove.performed += OnPointerMove;
        _action.Player.Attack.performed += (val) => _player.currentState?.HandleAttack(true);
        _action.Player.Attack.canceled += (val) => _player.currentState?.HandleAttack(false);
        _action.Player.Interact.performed += (val) => _player.currentState?.HandleInteract();
        _action.Player.Run.performed += (val) => _player.currentState?.HandleRun(true);
        _action.Player.Run.canceled += (val) => _player.currentState?.HandleRun(false);
        _action.Player.Flashlight.performed += (val) => _player.currentState?.HandleFlashlightPower();

        _action.Player.ChangeItem.performed += (val) => HandleScrollAbility(val.ReadValue<Vector2>());

        _action.Player.Crouch.performed += (val) => _player.currentState?.HandleCrouch(true);
        _action.Player.Crouch.canceled += (val) => _player.currentState?.HandleCrouch(false);


        _action.Enable();

    }
    public void DisableInput()
    {
        _action.Player.Move.performed -= (val) => Movement = val.ReadValue<Vector2>();
        _action.Player.PointerMove.performed -= OnPointerMove;
        _action.Player.Attack.performed -= (val) => _player.currentState?.HandleAttack(true);
        _action.Player.Attack.canceled -= (val) => _player.currentState?.HandleAttack(false);
        _action.Player.Interact.performed -= (val) => _player.currentState?.HandleInteract();
        _action.Player.Run.performed -= (val) => _player.currentState?.HandleRun(true);
        _action.Player.Run.canceled -= (val) => _player.currentState?.HandleRun(false);
        _action.Player.Flashlight.performed -= (val) => _player.currentState?.HandleFlashlightPower();

        _action.Player.ChangeItem.performed -= (val) => HandleScrollAbility(val.ReadValue<Vector2>());

        _action.Player.Crouch.performed -= (val) => _player.currentState?.HandleCrouch(true);
        _action.Player.Crouch.canceled -= (val) => _player.currentState?.HandleCrouch(false);


        _action.Disable();

    }
    private void HandleScrollAbility(Vector2 scrollValue)
    {
        if (scrollValue.y > 0 || scrollValue.x > 0)
        {
            _player.currentState?.HandleChangeAbility(1);
        }
        else if (scrollValue.y < 0 || scrollValue.x < 0)
        {
            _player.currentState?.HandleChangeAbility(-1);
        }
    }

    private void OnPointerMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        InputDevice device = context.control.device;
        _device = device;
        _lookAround = input;
    }

}
