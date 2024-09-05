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

    private void Update()
    {

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

        _action.Enable();

    }
    public void DisableInput()
    {
        _action.Player.Move.performed -= (val) => Movement = val.ReadValue<Vector2>();
        _action.Player.PointerMove.performed -= OnPointerMove;

        _action.Disable();

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
