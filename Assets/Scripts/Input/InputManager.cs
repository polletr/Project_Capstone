using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInput _action;

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

    Vector2 _direction;
    public Vector2 Direction
    {
        get
        {
            return _direction;
        }
        private set
        {
            _direction = value;
        }
    }


    void Awake()
    {
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
        _action.Player.PointerMove.performed += (val) => Direction = val.ReadValue<Vector2>();

        _action.Enable();

    }

    public void DisableInput()
    {
        _action.Player.Move.performed -= (val) => Movement = val.ReadValue<Vector2>();
        _action.Player.PointerMove.performed -= (val) => Direction = val.ReadValue<Vector2>();

        _action.Disable();

    }

}
