using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCinematicState : PlayerBaseState
{
    public PlayerCinematicState(PlayerController playerController) : base(playerController)
    {
    }


    public override void EnterState()
    {
        Debug.Log("Entering Cinematic State");
    }


    public override void StateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Player.ChangeState(Player.MoveState);
        }
    }

    public override void StateFixedUpdate()
    {
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Cinematic State");
    }

    public override void HandleLookAround(Vector2 dir, InputDevice device)
    {
        if (!Player.LookTarget || !Player.CanLookAround) return;

        var sensitivityMultiplier = Player.Settings.CinematicCameraSensitivityMouse;

        if (device is Gamepad)
        {
            sensitivityMultiplier = Player.Settings.cameraSensitivityGamepad;
        }

        // Calculate player's body (y-axis) rotation
        Player.yRotation += dir.x * sensitivityMultiplier * Time.deltaTime;

        // Clamp yRotation for cinematic view
        var yMinAngle = Player.Settings.CinematicClampAngleLeft;
        var yMaxAngle = Player.Settings.CinematicClampAngleRight;
        Player.yRotation = Mathf.Clamp(Player.yRotation, yMinAngle, yMaxAngle);
        Player.CameraHolder.localRotation = Quaternion.Euler(0, Player.yRotation, 0); // Rotate body horizontally

        // Calculate camera pitch (x-axis) rotation
        Player.xRotation += dir.y * sensitivityMultiplier * Time.deltaTime;

        // Clamp xRotation for vertical limits in cinematic view
        var xMinAngle = Player.Settings.CinematicClampAngleDown; // Max angle down
        var xMaxAngle = Player.Settings.CinematicClampAngleUp; // Max angle up
        Player.xRotation = Mathf.Clamp(Player.xRotation, xMinAngle, xMaxAngle);

        var currentRotation = Player.PlayerCam.transform.localEulerAngles;
        Player.PlayerCam.transform.localRotation = Quaternion.Euler(-Player.xRotation, 0, currentRotation.z);
    }


    public override void HandleAttack(bool held)
    {
    }

    public override void HandleDeath()
    {
    }

    public override void HandleFlashlightPower()
    {
    }

    public override void HandleInteract()
    {
    }


    public override void HandleMovement(Vector2 dir)
    {
    }

    public override void HandleMove()
    {
    }

    public override void HandleRecharge()
    {
    }
}