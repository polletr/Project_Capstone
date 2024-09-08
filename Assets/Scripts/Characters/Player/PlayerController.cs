using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour, IDamageable
{

    public PlayerSettings Settings { get { return settings; } }

    public GameEvent Event;

    public Vector3 AimPosition { get; private set; }
    public Quaternion PlayerRotation { get; private set; }

    public CharacterController characterController { get; set; }

    [HideInInspector]
    public PlayerBaseState currentState;

    [SerializeField]
    private PlayerSettings settings;

    private InputManager inputManager;
    private LayerMask groundLayer;

    private float health;

    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        inputManager = GetComponent<InputManager>();
        characterController = GetComponent<CharacterController>();
        health = Settings.PlayerHealth;
        ChangeState(new PlayerMoveState());
    }

    private void Update()
    {
        HandleMove(inputManager.Movement);
        currentState?.StateUpdate();
    }
    private void FixedUpdate() => currentState?.StateFixedUpdate();

    public void GetDamaged(float attackDamage)
    {
        health -= attackDamage;
        Debug.Log(health);
    }

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

    public void HandleGetHit()
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
    public void ChangeState(PlayerBaseState newState)
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
