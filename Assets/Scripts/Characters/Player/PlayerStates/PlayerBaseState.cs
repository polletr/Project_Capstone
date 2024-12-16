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

    protected CameraController CamController;

    protected float BobTimer;

    private Vector3 direction;

    protected PlayerBaseState(PlayerController playerController)
    {
        Player = playerController;
        PlayerAnimator = Player.playerAnimator;
        InputManager = Player.inputManager;
        CamController = Player.camController;
    }


    public virtual void EnterState()
    {
    }

    public virtual void ExitState()
    {
    }

    public virtual void StateFixedUpdate()
    {
    }

    public virtual void StateUpdate()
    {
        if(direction.magnitude != 0) 
            Player.characterController.SimpleMove(direction.normalized * GetSpeed());

        StepsSound();

        HandleFlashlightSphereCast();

        CheckInteractionUI();

        // Calculate the local movement direction relative to the player's forward direction
        var localDirection = Player.transform.InverseTransformDirection(direction);

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

        if (Player.DynamicMov)
            TiltingCamera(dir);
    }

    private void TiltingCamera(Vector2 dir)
    {
        // Calculate target tilt based on horizontal movement input
        float targetTilt = 0f;

        // Add oscillating tilt for forward movement
        if (dir != Vector2.zero)
        {
            // Calculate oscillation based on forward movement
            float oscillation = Mathf.Sin(Time.time * Player.Settings.SwayFrequency) * Player.Settings.SwayAmplitude;
            targetTilt += oscillation;
        }
        else
        {
            targetTilt = 0; // Reset tilt when not moving
        }

        // Smoothly interpolate the z-axis tilt
        float smoothTilt = Mathf.LerpAngle(Player.PlayerCam.transform.localEulerAngles.z, targetTilt, Player.Settings.TiltSpeed * Time.deltaTime);

        // Update camera's local rotation on Z-axis for tilt effect
        Player.PlayerCam.transform.localRotation = Quaternion.Euler(
            Player.PlayerCam.transform.localEulerAngles.x,
            Player.PlayerCam.transform.localEulerAngles.y,
            smoothTilt
        );

        // Call the bobbing method
        ApplyBobbing(dir);
    }

    private void ApplyBobbing(Vector2 dir)
    {
        // Create a bobbing effect based on the movement speed and direction
        if (dir.magnitude > 0)
        {
            float bobbingAmount = Mathf.Sin(Time.time * Player.Settings.BobFrequency) * Player.Settings.BobAmplitude;
            Vector3 bobOffset = new Vector3(0, bobbingAmount, 0); // Only apply bobbing on the Y-axis

            // Apply the bobbing effect to the camera position
            Player.PlayerCam.transform.localPosition = new Vector3(
                Player.PlayerCam.transform.localPosition.x,
                Player.PlayerCam.transform.localPosition.y + bobOffset.y,
                Player.PlayerCam.transform.localPosition.z
            );
        }
        else
        {
            // Reset to the original position when not moving
            Player.PlayerCam.transform.localPosition = new Vector3(
                Player.PlayerCam.transform.localPosition.x,
                Mathf.Lerp(Player.PlayerCam.transform.localPosition.y, 0f, Player.Settings.BobResetSpeed * Time.deltaTime),
                Player.PlayerCam.transform.localPosition.z
            );
        }
    }
    public virtual void HandleRecharge() { }

    public virtual void HandleAttack(bool held)
    {
        if (held)
        {
            Player.ChangeState(Player.AttackState);
        }
    }

    public virtual void HandleMove()
    {
    }

    public virtual void HandleInteract()
    {
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


    private Quaternion flashlightRotationOffset = Quaternion.Euler(-180, -90, 0);

    public virtual void HandleLookAround(Vector2 dir, InputDevice device)
    {
        var sensitivityMultiplier = Player.Settings.cameraSensitivityMouse;

        if (device is Gamepad)
        {
            sensitivityMultiplier = Player.Settings.cameraSensitivityGamepad;
        }

        // Calculate player's body (y-axis) rotation
        Player.yRotation += dir.x * sensitivityMultiplier * Time.deltaTime;
        Player.CameraHolder.localRotation = Quaternion.Euler(0, Player.yRotation, 0); // Rotate body horizontally

        // Calculate camera pitch (x-axis) rotation
        Player.xRotation += dir.y * sensitivityMultiplier * Time.deltaTime;
        Player.xRotation = Mathf.Clamp(Player.xRotation, Player.Settings.ClampAngleUp, Player.Settings.ClampAngleDown);
        Vector3 currentRotation = Player.PlayerCam.transform.localEulerAngles;

        // Set the new rotation, keeping the current z rotation
        Player.PlayerCam.transform.localRotation = Quaternion.Euler(-Player.xRotation, 0, currentRotation.z);

        // Only update the flashlight's rotation if the player is holding it
        if (Player.HasFlashlight)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Player.PlayerCam.transform.forward) * flashlightRotationOffset;


            if (Player.xRotation < Player.Settings.FlashlightAngleDown)
            {
                targetRotation = Quaternion.LookRotation(Player.CameraHolder.transform.forward) * flashlightRotationOffset;
                Player.flashlight.SetLightSettings(Player.flashlight.FlashlightHitPos,false);
            }
            else
            {
                Player.flashlight.SetLightSettings(Player.flashlight.FlashlightHitPos);
            }


            // Smoothly rotate the hand towards the target rotation with the specified delay
            Player.Hand.rotation = Quaternion.Slerp(Player.Hand.rotation, targetRotation, Player.Settings.flashlightFollowDelay * Time.deltaTime);
        }
    }
    public virtual void HandleDeath()
    {
        Player.ChangeState(Player.DeathState);
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


    public virtual void CheckInteractionUI()
    {
        if (Physics.Raycast(Player.PlayerCam.transform.position, Player.PlayerCam.transform.forward, out RaycastHit hit,
                Player.Settings.InteractionRange))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out Interactable thing))
            {
                Player.interactableObj = thing;
                if (Player.interactableObj.indicatorHandler != null &&
                    Player.interactableObj.indicatorHandler.IndicatorUI != null)
                    Player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(true);
            }
            else
            {
                if (Player.interactableObj != null && Player.interactableObj.indicatorHandler != null &&
                    Player.interactableObj.indicatorHandler.IndicatorUI != null)
                    Player.interactableObj.indicatorHandler.IndicatorUI.TriggerTextIndicator(false);
            }
        }
        else
        {
            if (Player.interactableObj != null && Player.interactableObj.indicatorHandler != null &&
                Player.interactableObj.indicatorHandler.IndicatorUI != null)
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