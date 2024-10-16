using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerBaseState
{
    protected Vector3 _direction;

    protected bool isRunning;
    protected bool isCrouching;

    public PlayerController player { get; set; }
    public PlayerAnimator playerAnimator { get; set; }
    public InputManager inputManager { get; set; }

    protected float _bobTimer;


    public PlayerBaseState(PlayerController playerController)
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
        if (player.HasFlashlight)
            player.flashlight.HandleSphereCast();
    }

    public virtual void HandleFlashlightPower()
    {
        if (player.HasFlashlight)
            player.flashlight.HandleFlashlightPower();
    }


    public virtual void HandleLookAround(Vector2 dir, InputDevice device)
    {
        float sensitivityMult = player.Settings.cameraSensitivityMouse;

        if (device is Gamepad)
        {
            sensitivityMult = player.Settings.cameraSensitivityGamepad;
        }

        // Calculate player's body (y-axis) rotation
        player.yRotation += dir.x * sensitivityMult * Time.deltaTime;
        player.CameraHolder.localRotation = Quaternion.Euler(0, player.yRotation, 0);  // Rotate body horizontally

        // Calculate camera pitch (x-axis) rotation
        player.xRotation += dir.y * sensitivityMult * Time.deltaTime;
        player.xRotation = Mathf.Clamp(player.xRotation, player.Settings.ClampAngleUp, player.Settings.ClampAngleDown);
        player.PlayerCam.transform.localRotation = Quaternion.Euler(-player.xRotation, 0, 0);  // Rotate camera vertically

        // Only update the flashlight's rotation if the player is holding it
        if (player.HasFlashlight && player.xRotation > player.Settings.FlashlightAngleDown)
        {
            // Get current rotation and target rotation in Euler angles
            Vector3 currentRotation = player.Hand.localRotation.eulerAngles;
            Vector3 targetRotation = player.PlayerCam.transform.rotation.eulerAngles;

            // Slerp only the z-axis, while keeping the x and y axes unchanged
            float zRotation = Mathf.LerpAngle(currentRotation.z, targetRotation.x, player.Settings.FlashlightRotateSpeed * Time.deltaTime);

            // Apply the new rotation, only modifying the z-axis
            player.Hand.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, zRotation);
        }
    }


    protected virtual void StepsSound()
    {
        PLAYBACK_STATE playbackState;
        player.playerFootsteps.getPlaybackState(out playbackState);

        if (_direction.sqrMagnitude > 0f)
        {
            if (player.Event.OnSoundEmitted != null)
                player.Event.OnSoundEmitted.Invoke(player.transform.position, GetSoundEmitted());
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
                player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(true);
            }
            else
            {
                if (player.interactableObj != null)
                    player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(false);
            }
        }
        else
        {
            if (player.interactableObj != null)
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

