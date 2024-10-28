using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerBaseState
{
    protected bool IsRunning;
    protected bool IsCrouching;

    protected PlayerController Player { get; set; }
    protected PlayerAnimator PlayerAnimator { get; set; }
    public InputManager InputManager { get; set; }

    protected float BobTimer;

    private Vector3 direction;

    protected PlayerBaseState(PlayerController playerController)
    {
        Player = playerController;
        PlayerAnimator = Player.playerAnimator;
        InputManager = Player.inputManager;
    }


    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate()
    {
        Player.characterController.SimpleMove(direction.normalized * GetSpeed());

        StepsSound();

        HandleFlashlightSphereCast();

        CheckInteractionUI();

        // Calculate the local movement direction relative to the player's forward direction
        var localDirection = Player.transform.InverseTransformDirection(direction);

        // Set the animator parameters based on the local direction
        PlayerAnimator.animator.SetFloat("Horizontal", localDirection.x * GetMovementAnimValue());
        PlayerAnimator.animator.SetFloat("Vertical", localDirection.z * GetMovementAnimValue());

    }

    public virtual void HandleMovement(Vector2 dir)
    {

        // Get the camera's forward and right directions
        var cameraForward = Player.PlayerCam.transform.forward;
        var cameraRight = Player.PlayerCam.transform.right;

        // Since we're working in a 3D space, we don't want any vertical movement on the Y axis
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the vectors so we get consistent movement regardless of camera angle
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction based on the input (dir) and camera orientation
        direction = (cameraForward * dir.y + cameraRight * dir.x);

        // Calculate target tilt based on horizontal movement input
        float targetTilt = -dir.x * Player.Settings.TiltAngle;

        // Add oscillating tilt for forward movement
        if (direction != Vector3.zero && Mathf.Abs(Vector3.Dot(direction.normalized, Player.PlayerCam.transform.forward)) > 0.1f)
        {
            float oscillation = Mathf.Sin(Time.time * Player.Settings.SwayFrequency) * Player.Settings.SwayAmplitude;
            targetTilt += oscillation;
        }
        else
        {
            targetTilt = 0;
        }    

        // Smoothly interpolate the z-axis tilt
        float smoothTilt = Mathf.LerpAngle(Player.PlayerCam.transform.localEulerAngles.z, targetTilt, Player.Settings.TiltSpeed * Time.deltaTime);

        // Update camera's local rotation on Z-axis for tilt effect
        Player.PlayerCam.transform.localRotation = Quaternion.Euler(
            Player.PlayerCam.transform.localEulerAngles.x,
            Player.PlayerCam.transform.localEulerAngles.y,
            smoothTilt
        );

    }
    
    public virtual void HandleRecharge()
    {
        
    }

    public virtual void HandleAttack(bool held)
    {
            Player.ChangeState(Player.AttackState);
    }

    public virtual void HandleMove() { }

    public virtual void HandleInteract() { }
    
    public virtual void HandleRun(bool check)
    {
        if (!IsCrouching)
            IsRunning = check;
    }

    public virtual void HandleCrouch(bool check)
    {
        IsCrouching = check;
        PlayerAnimator.animator.SetBool("isCrouching", check);

        if (IsCrouching)
            IsRunning = false;
    }

    protected virtual void HandleFlashlightSphereCast()
    {
        if (Player.HasFlashlight)
            Player.flashlight.HandleRayCast();
    }

    public virtual void HandleFlashlightPower()
    {
        if (Player.HasFlashlight)
            Player.flashlight.HandleFlashlightPower();
    }
    
    public virtual void HandleLookAround(Vector2 dir, InputDevice device)
    {
        var sensitivityMultilayer = Player.Settings.cameraSensitivityMouse;

        if (device is Gamepad)
        {
            sensitivityMultilayer = Player.Settings.cameraSensitivityGamepad;
        }

        // Calculate player's body (y-axis) rotation
        Player.yRotation += dir.x * sensitivityMultilayer * Time.deltaTime;
        Player.CameraHolder.localRotation = Quaternion.Euler(0, Player.yRotation, 0);  // Rotate body horizontally

        // Calculate camera pitch (x-axis) rotation
        Player.xRotation += dir.y * sensitivityMultilayer * Time.deltaTime;
        Player.xRotation = Mathf.Clamp(Player.xRotation, Player.Settings.ClampAngleUp, Player.Settings.ClampAngleDown);
        Player.PlayerCam.transform.localRotation = Quaternion.Euler(-Player.xRotation, 0, 0);  // Rotate camera vertically

        // Only update the flashlight's rotation if the player is holding it
        if (Player.HasFlashlight && Player.xRotation > Player.Settings.FlashlightAngleDown)
        {
            // Get current rotation and target rotation in Euler angles
            var currentRotation = Player.Hand.localRotation.eulerAngles;
            var targetRotation = Player.PlayerCam.transform.rotation.eulerAngles;

            // Slurp only the z-axis, while keeping the x and y axes unchanged
            var zRotation = Mathf.LerpAngle(currentRotation.z, targetRotation.x, Player.Settings.FlashlightRotateSpeed * Time.deltaTime);

            // Apply the new rotation, only modifying the z-axis
            Player.Hand.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, zRotation);
        }
    }


    protected virtual void StepsSound()
    {
        Player.playerFootsteps.getPlaybackState(out var playbackState);

        if (direction.sqrMagnitude > 0f)
        {
            if (Player.Event.OnSoundEmitted != null)
                Player.Event.HandlePlayerFootSteps(Player.transform.position, GetSoundEmitted());

            //Actual sound
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                Player.playerFootsteps.start();
            }
        }
        else
        {
            if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
            {
                Player.playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    public virtual void HandleDeath()
    {
        Player.ChangeState(Player.DeathState);
    }

    public virtual void CheckInteractionUI()
    {
        if (Physics.Raycast(Player.PlayerCam.transform.position, Player.PlayerCam.transform.forward, out RaycastHit hit, Player.Settings.InteractionRange))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out Interactable thing))
            {
                Player.interactableObj = thing;
                if (Player.interactableObj.indicatorHandler != null && Player.interactableObj.indicatorHandler.IndicatorUI != null)
                    Player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(true);
            }
            else
            {
                if (Player.interactableObj != null && Player.interactableObj.indicatorHandler != null && Player.interactableObj.indicatorHandler.IndicatorUI != null)
                    Player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(false);
            }
        }
        else
        {
            if (Player.interactableObj != null && Player.interactableObj.indicatorHandler != null && Player.interactableObj.indicatorHandler.IndicatorUI != null)
            {
                Player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(false);
                Player.interactableObj = null;
            }
        }

    }

    protected virtual float GetSpeed()
    {
        if (IsRunning)
        {
            return Player.Settings.RunningSpeed;
        }
        else if (IsCrouching)
        {
            return Player.Settings.CrouchingSpeed;
        }
        else
        {
            return Player.Settings.WalkingSpeed;
        }
    }

    protected virtual float GetSoundEmitted()
    {
        if (IsRunning)
        {
            return Player.Settings.RunSoundRange;
        }
        else if (IsCrouching)
        {
            return Player.Settings.CrouchSoundRange;
        }
        else
        {
            return Player.Settings.WalkSoundRange;
        }
    }

    protected virtual float GetMovementAnimValue()
    {
        if (IsRunning || IsCrouching)
        {
            return 1f;
        }
        else
        {
            return 0.5f;
        }
    }

}

