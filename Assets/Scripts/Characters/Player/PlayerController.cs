using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    private InputManager inputManager;
    public Vector3 AimPosition { get; private set; }
    public Quaternion PlayerRotation { get; private set; }
    private LayerMask groundLayer;

    public CharacterController characterController { get; set; }

    [HideInInspector]
    public PlayerBaseState currentState;

    [SerializeField]
    private PlayerSettings settings;
    public PlayerSettings Settings { get { return settings; } }

    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        inputManager = GetComponent<InputManager>();
        characterController = GetComponent<CharacterController>();
        ChangeState(new PlayerMoveState());
    }

    private void Update()
    {
        HandleMove(inputManager.Movement);
        currentState?.StateUpdate();
    }
    private void FixedUpdate() => currentState?.StateFixedUpdate();

    #region Character Actions
    public void HandleMove(Vector2 dir)
    {
        currentState?.HandleMovement(dir);
    }

    public void HandleInteract()
    {

    }

    public void CancelInteract()
    {

    }

    public void HandleAttack()
    {

    }
    public void HandleMouseInput(Vector2 input)
    {
        Ray ray = Camera.main.ScreenPointToRay(input);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 target = hit.point;
            Vector3 direction = target - transform.position;
            direction.y = 0; // Keep the character level on the Y-axis
            PlayerRotation = Quaternion.LookRotation(direction);
        }
    }
    public void HandleGamepadInput(Vector2 input)
    {
        // Logic to handle gamepad input
        Vector3 inputDirection = new Vector3(input.x, 0, input.y);

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            PlayerRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
        }
    }


    #endregion

    #region ChangeState
    public virtual void ChangeState(PlayerBaseState newState)
    {
        StartCoroutine(WaitFixedFrame(newState));
    }

    private IEnumerator WaitFixedFrame(PlayerBaseState newState)
    {

        yield return new WaitForFixedUpdate();
        currentState?.ExitState();
        currentState = newState;
        currentState.player = this;
        currentState.EnterState();

    }
    #endregion


}
