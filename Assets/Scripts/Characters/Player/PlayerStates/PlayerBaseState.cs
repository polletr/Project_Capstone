using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerBaseState
{
    protected bool isRunning;
    protected bool isCrouching;

    protected PlayerController player { get; set; }
    protected PlayerAnimator playerAnimator { get; set; }
    public InputManager inputManager { get; set; }

    protected float _bobTimer;

    private Vector3 _direction;

    protected PlayerBaseState(PlayerController playerController)
    {
        player = playerController;
        playerAnimator = player.playerAnimator;
        inputManager = player.inputManager;
    }


    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate()
    {
        player.characterController.SimpleMove(_direction.normalized * GetSpeed());

        StepsSound();

        HandleFlashlightSphereCast();

        CheckInteractionUI();

        // Calculate the local movement direction relative to the player's forward direction
        Vector3 localDirection = player.transform.InverseTransformDirection(_direction);

        // Set the animator parameters based on the local direction
        playerAnimator.animator.SetFloat("Horizontal", localDirection.x * GetMovementAnimValue());
        playerAnimator.animator.SetFloat("Vertical", localDirection.z * GetMovementAnimValue());

    }

    public virtual void HandleMovement(Vector2 dir)
    {

        // Get the camera's forward and right directions
        Vector3 cameraForward = player.PlayerCam.transform.forward;
        Vector3 cameraRight = player.PlayerCam.transform.right;

        // Since we're working in a 3D space, we don't want any vertical movement on the Y axis
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the vectors so we get consistent movement regardless of camera angle
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction based on the input (dir) and camera orientation
        _direction = (cameraForward * dir.y + cameraRight * dir.x);
    }
    
    public virtual void HandleRecharge()
    {
        
    }

    public virtual void HandleAttack(bool isHeld)
    {
        if (isHeld)
            player.ChangeState(player.AttackState);
    }

    public virtual void HandleMove() { }

    public virtual void HandleInteract() { }
    
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

    protected virtual void HandleFlashlightSphereCast()
    {
        if (player.HasFlashlight)
            player.flashlight.HandleRayCast();
    }

    public virtual void HandleFlashlightPower()
    {
        if (player.HasFlashlight)
            player.flashlight.HandleFlashlightPower();
    }


    public virtual void HandleLookAround(Vector2 dir, InputDevice device)
    {
        var sensitivityMultilayer = player.Settings.cameraSensitivityMouse;

        if (device is Gamepad)
        {
            sensitivityMultilayer = player.Settings.cameraSensitivityGamepad;
        }

        // Calculate player's body (y-axis) rotation
        player.yRotation += dir.x * sensitivityMultilayer * Time.deltaTime;
        player.CameraHolder.localRotation = Quaternion.Euler(0, player.yRotation, 0);  // Rotate body horizontally

        // Calculate camera pitch (x-axis) rotation
        player.xRotation += dir.y * sensitivityMultilayer * Time.deltaTime;
        player.xRotation = Mathf.Clamp(player.xRotation, player.Settings.ClampAngleUp, player.Settings.ClampAngleDown);
        player.PlayerCam.transform.localRotation = Quaternion.Euler(-player.xRotation, 0, 0);  // Rotate camera vertically

        // Only update the flashlight's rotation if the player is holding it
        if (player.HasFlashlight && player.xRotation > player.Settings.FlashlightAngleDown)
        {
            // Get current rotation and target rotation in Euler angles
            var currentRotation = player.Hand.localRotation.eulerAngles;
            var targetRotation = player.PlayerCam.transform.rotation.eulerAngles;

            // Slurp only the z-axis, while keeping the x and y axes unchanged
            var zRotation = Mathf.LerpAngle(currentRotation.z, targetRotation.x, player.Settings.FlashlightRotateSpeed * Time.deltaTime);

            // Apply the new rotation, only modifying the z-axis
            player.Hand.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, zRotation);
        }
    }


    protected virtual void StepsSound()
    {
        player.playerFootsteps.getPlaybackState(out var playbackState);

        if (_direction.sqrMagnitude > 0f)
        {
            if (player.Event.OnSoundEmitted != null)
                player.Event.HandlePlayerFootSteps(player.transform.position, GetSoundEmitted());

            //Actual sound
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                player.playerFootsteps.start();
            }
        }
        else
        {
            if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
            {
                player.playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    public virtual void HandleDeath()
    {
        player.ChangeState(player.DeathState);
    }

    public virtual void CheckInteractionUI()
    {
        if (Physics.Raycast(player.PlayerCam.transform.position, player.PlayerCam.transform.forward, out RaycastHit hit, player.Settings.InteractionRange))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out Interactable thing))
            {
                player.interactableObj = thing;
                if (player.interactableObj.indicatorHandler != null && player.interactableObj.indicatorHandler.IndicatorUI != null)
                    player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(true);
            }
            else
            {
                if (player.interactableObj != null && player.interactableObj.indicatorHandler != null && player.interactableObj.indicatorHandler.IndicatorUI != null)
                    player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(false);
            }
        }
        else
        {
            if (player.interactableObj != null && player.interactableObj.indicatorHandler != null && player.interactableObj.indicatorHandler.IndicatorUI != null)
            {
                player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(false);
                player.interactableObj = null;
            }
        }

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

