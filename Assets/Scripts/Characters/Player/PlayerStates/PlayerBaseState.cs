using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBaseState : MonoBehaviour
{

    protected static readonly int IdleHash = Animator.StringToHash("Idle");
    protected static readonly int RunHash = Animator.StringToHash("Run");
    protected static readonly int WalkHash = Animator.StringToHash("Walk");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int GetHitHash = Animator.StringToHash("GetHit");
    protected static readonly int DieHash = Animator.StringToHash("Die");

    protected Vector3 _direction;

    protected bool isRunning;
    protected bool isCrouching;

    public PlayerController player { get; set; }
    public InputManager inputManager { get; set; }
    public virtual void EnterState()
    {

    }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate()
    {

    }
    public virtual void StateUpdate() 
    {
        Rotate();
        player.characterController.SimpleMove(_direction.normalized * GetSpeed());

        if (player.Event.OnSoundEmitted != null)
        {
            if (_direction.sqrMagnitude > 0f)
            {
                player.Event.OnSoundEmitted.Invoke(player.transform.position, GetSoundEmitted());
            }
        }

        // Calculate the local movement direction relative to the player's forward direction
        Vector3 localDirection = player.transform.InverseTransformDirection(_direction);

        // Set the animator parameters based on the local direction
        player.animator.SetFloat("Horizontal", localDirection.x * GetMovementAnimValue());
        player.animator.SetFloat("Vertical", localDirection.z * GetMovementAnimValue());

    }

    public virtual void HandleMovement(Vector2 dir) 
    {
        // Get the camera's forward direction (in the XZ plane)
        Vector3 cameraForward = player.IsoFollowCamera.transform.forward;
        cameraForward.y = 0f; // Ignore the Y component for horizontal movement
        cameraForward.Normalize();

        // Get the camera's right direction
        Vector3 cameraRight = player.IsoFollowCamera.transform.right;
        cameraRight.y = 0f; // Ignore the Y component for horizontal movement
        cameraRight.Normalize();

        // Convert the input direction to world space relative to the camera
        _direction = (cameraForward * dir.y + cameraRight * dir.x).normalized;

    }

    public virtual void HandleAttack()
    {
        player.ChangeState(new PlayerAttackState());
    }

    public virtual void HandleInteract()
    {
        player.ChangeState(new PlayerInteractState());

    }

    public virtual void HandleEquipItem(IInventoryItem item) { }

    public virtual void HandleGetHit()
    {
        player.ChangeState(new PlayerGetHitState());

    }

    public virtual void HandleChangeItem(int scrollDirection)
    {

    }

    public virtual void HandleRun(bool check)
    {
        if (!isCrouching)
            isRunning = check;
    }

    public virtual void HandleCrouch(bool check)
    {
        isCrouching = check;
        player.animator.SetBool("isCrouching", check);

        if (isCrouching)
            isRunning = false;
    }


    public virtual void StopInteract() { }

    public virtual void HandleDropItem() { }

    public virtual void CancelDropItem() { }



    public virtual void HandleDeath() 
    {
        player.ChangeState(new PlayerDeathState());
    }

    protected void Rotate()
    {
        player.gameObject.transform.rotation = Quaternion.Slerp(player.gameObject.transform.rotation, player.PlayerRotation, player.Settings.RotationSpeed);
    }

    private float GetSpeed()
    {
        if (isRunning)
        {
            return player.Settings.RunningSpeed;
        }
        else if (isCrouching)
        {
            return player.Settings.CrouchingSpeed;
        }
        else
        {
            return player.Settings.WalkingSpeed;
        }
    }

    private float GetSoundEmitted()
    {
        if (isRunning)
        {
            return player.Settings.RunSoundRange;
        }
        else if (isCrouching)
        {
            return player.Settings.CrouchSoundRange;
        }
        else
        {
            return player.Settings.WalkSoundRange;
        }
    }

    private float GetMovementAnimValue()
    {
        if (isRunning || isCrouching)
        {
            return 1f;
        }
        else
        {
            return 0.5f;
        }
    }

}

