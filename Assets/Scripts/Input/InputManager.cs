using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //_action.Player.PointerMove.performed += (val) => _player.HandlePointerDirection(val.ReadValue<Vector2>());

        _action.Enable();

    }
    public void DisableInput()
    {
        _action.Player.Move.performed -= (val) => Movement = val.ReadValue<Vector2>();
        //_action.Player.PointerMove.performed -= (val) => _player.HandlePointerDirection(val.ReadValue<Vector2>());

        _action.Disable();

    }

}
