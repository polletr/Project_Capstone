using System.Collections;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class PlayerController : MonoBehaviour, IDamageable
{

    public PlayerSettings Settings { get { return settings; } }

    public GameEvent Event;
    public Inventory inventory;


    public Vector3 AimPosition { get; private set; }
    public Camera IsoFollowCamera { get { return isoFollowCamera; } }

    public Quaternion PlayerRotation { get; private set; }
    public float Health { get; private set; }

    public CharacterController characterController { get; set; }
    public Animator animator { get; set; }

    [HideInInspector]
    public PlayerBaseState currentState;

    [SerializeField]
    private PlayerSettings settings;
    [SerializeField]
    private Camera isoFollowCamera;

    private InputManager inputManager;
    private LayerMask groundLayer;

    public IInventoryItem interactableItem { get; set; }
    public IInventoryItem currentItemEquipped { get; set; }

    [SerializeField]
    private GameObject meleeSocketHand;

    public GameObject MeleeSocketHand { get { return meleeSocketHand; } }


    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        inputManager = GetComponent<InputManager>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        Health = Settings.PlayerHealth;
        currentItemEquipped = null;
        Event.OnItemEquipped += HandleEquipItem;
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
        Health -= attackDamage;
        if (Health > 0f)
        {
            currentState?.HandleGetHit();
        }
        else
        {
            currentState?.HandleDeath();
        }
        
    }
    
    public void MeleeAttack()
    {
        if (currentItemEquipped != null) 
        {
            if (currentItemEquipped is Weapon)
            {
                // Perform the raycast
                RaycastHit hit;
                Vector3 forwardDirection = transform.forward;
                Vector3 startPosition = transform.position;

                // Raycast in the forward direction from the player's position
                if (Physics.Raycast(startPosition, forwardDirection, out hit, (currentItemEquipped as Weapon).AttackRange))
                {
                    // Check if the raycast hit an enemy
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        hit.collider.GetComponent<EnemyClass>().GetDamaged((currentItemEquipped as Weapon).Damage);
                        // Execute your attack logic here
                    }
                }
            }

        }
    }

    #region Character Actions
    public void HandleMove(Vector2 dir)
    {
        currentState?.HandleMovement(dir);
    }

    public void HandleInteract()
    {
        currentState?.HandleInteract();
    }

    public void HandleEquipItem(IInventoryItem item)
    {
        currentState?.HandleEquipItem(item);
    }

    public void HandleChangeItem(int direction)
    {
        currentState?.HandleChangeItem(direction);
    }

    public void CancelInteract()
    {

    }

    public void HandleDropItem()
    {
        currentState?.HandleDropItem();
    }
    public void CancelDropItem()
    {
        currentState?.CancelDropItem();
    }


    public void HandleAttack()
    {
        currentState?.HandleAttack();
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
        if (item != null)
        {
            // Store the interactable item reference
            interactableItem = item;
            Debug.Log(interactableItem);
            // Visual feedback, like highlighting the item
            // e.g., item.Highlight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the player leaves the item's area, clear the reference
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null && item == interactableItem)
        {
            // Reset interaction reference and visual feedback
            interactableItem = null;
            // e.g., item.Highlight(false);
        }
    }


    #region ChangeState
    public void ChangeState(PlayerBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.player = this;
        currentState.EnterState();
    }

/*    private IEnumerator WaitFixedFrame(PlayerBaseState newState)
    {

        yield return new WaitForFixedUpdate();

    }
*/    
    #endregion


}
