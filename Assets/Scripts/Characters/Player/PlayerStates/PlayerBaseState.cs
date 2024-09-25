using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerBaseState 
{
    protected Vector3 _direction;

    protected bool isRunning;
    protected bool isCrouching;

    public PlayerAnimator playerAnimator { get; set; }
    public PlayerController player { get; set; }
    
    public InputManager inputManager { get; set; }
  
    public PlayerBaseState(PlayerAnimator animator, PlayerController playerController, InputManager inputM)
    {
        playerAnimator = animator;
        player = playerController;
        inputManager = inputM;
    }

  
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate()
    {
        player.characterController.SimpleMove(_direction.normalized * GetSpeed());

        if (player.Event.OnSoundEmitted != null)
        {
            if (_direction.sqrMagnitude > 0f)
            {
                player.Event.OnSoundEmitted.Invoke(player.transform.position, GetSoundEmitted());
            }
        }

        HandleFlashlightSphereCast();

        // Calculate the local movement direction relative to the player's forward direction
        Vector3 localDirection = player.transform.InverseTransformDirection(_direction);

        // Set the animator parameters based on the local direction
        playerAnimator.animator.SetFloat("Horizontal", localDirection.x * GetMovementAnimValue());
        playerAnimator.animator.SetFloat("Vertical", localDirection.z * GetMovementAnimValue());

    }

    public virtual void HandleMovement(Vector2 dir)
    {

        // Get the camera's forward and right directions
        Vector3 cameraForward = player.Camera.forward;
        Vector3 cameraRight = player.Camera.right;

        // Since we're working in a 3D space, we don't want any vertical movement on the Y axis
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the vectors so we get consistent movement regardless of camera angle
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction based on the input (dir) and camera orientation
        _direction = (cameraForward * dir.y + cameraRight * dir.x);
    }

    public virtual void HandleAttack(bool isHeld)
    {
        if (isHeld)
            player.ChangeState(player.AttackState);
    }

    public virtual void HandleMove() { }

    public virtual void HandleInteract()
    {

    }

    public virtual void HandleChangeAbility(int direction) { }

    public virtual void HandleEquipItem(IInventoryItem item) { }

    public virtual void HandleGetHit()
    {
        player.ChangeState(player.GetHitState);

    }

    public virtual void HandleRun(bool check)
    {
        if (!isCrouching)
            isRunning = check;
    }

    public virtual void HandleCrouch(bool check)
    {
        isCrouching = check;
        playerAnimator.animator.SetBool("isCrouching", check);

        if (isCrouching)
            isRunning = false;
    }
    
    public virtual void HandleFlashlightSphereCast()
    {
        player.flashlight.HandleSphereCast();
    }

    public virtual void HandleFlashlightPower()
    {
        player.flashlight.HandleFlashlightPower();
    }


    public virtual void HandleLookAround(Vector2 dir, InputDevice device)
    {
        float sensitivityMult = player.Settings.cameraSensitivityMouse;

        if (device is Gamepad)
        {
            sensitivityMult = player.Settings.cameraSensitivityGamepad;
        }

        player.xRotation += dir.y * sensitivityMult * Time.deltaTime;
        player.xRotation = Mathf.Clamp(player.xRotation, -90f, 90f);
        player.yRotation += dir.x * sensitivityMult * Time.deltaTime;

        player.Hand.localRotation = Quaternion.Euler(-player.xRotation, player.yRotation, 0);

        player.CameraHolder.localRotation = Quaternion.Lerp(player.CameraHolder.localRotation, Quaternion.Euler(0, player.yRotation, 0), player.Settings.cameraAcceleration * Time.deltaTime);
        player.Camera.localRotation = Quaternion.Lerp(player.Camera.localRotation, Quaternion.Euler(-player.xRotation, 0, 0), player.Settings.cameraAcceleration * Time.deltaTime);

    }

    public virtual void HandleDeath()
    {
        player.ChangeState(player.DeathState);
    }

    protected virtual float GetSpeed()
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

    protected virtual float GetSoundEmitted()
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

    protected virtual float GetMovementAnimValue()
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

