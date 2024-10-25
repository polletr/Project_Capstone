using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{

    public PlayerMoveState(PlayerController playerController) : base(playerController) { }

    private Vector2 _dir = Vector2.zero;

    public override void EnterState()
    {
        PlayerAnimator.animator.Play(PlayerAnimator.IdleHash);
    }
    public override void ExitState()
    {
      
    }

    public override void StateFixedUpdate() { }

    public override void StateUpdate()
    {
        base.StateUpdate();

        HandleBobing();

        Ray ray = new Ray(Player.PlayerCam.transform.position, Player.PlayerCam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Player.Settings.MaxEnemyDistance))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out EnemyClass enemy))
            {
                Player.AddEnemyToChaseList(enemy);
            }
        }

    }
    

    public override void HandleInteract()
    {
        if (Player.interactableObj != null)
            Player.ChangeState(Player.InteractState);
    }

    public override void HandleRecharge()
    {
        Player.flashlight.ZeroOutBattery();
        Player.ChangeState(Player.RechargeState);
    }
    
    private void HandleBobing()
    {
        // Check if the player is moving
        if (Player.characterController.velocity.magnitude > 0.1f)
        {
            // Increment bobbing timer based on time and frequency
            BobTimer += Time.deltaTime * (IsRunning ? Player.Settings.BobSpeedRun : IsCrouching ? Player.Settings.BobSpeedCrouch : Player.Settings.BobSpeedWalk);

            // Calculate vertical bobbing offset using sine wave
            float bobOffsetY = Mathf.Sin(BobTimer) * Player.Settings.BobAmount; // Use BobAmount for vertical bobbing

            // Apply the calculated bobbing offset to the camera's local position
            Player.PlayerCam.transform.localPosition = new Vector3(
                Player.DefaultCameraLocalPosition.x,  // No left/right bobbing
                Player.DefaultCameraLocalPosition.y + bobOffsetY,  // Up/down bobbing
                Player.DefaultCameraLocalPosition.z
            );
        }
        else
        {
            // Smoothly return the camera to its default position when not moving
            Player.PlayerCam.transform.localPosition = Vector3.Lerp(
                Player.PlayerCam.transform.localPosition,
                Player.DefaultCameraLocalPosition,
                Time.deltaTime * (IsRunning ? Player.Settings.BobSpeedRun : IsCrouching ? Player.Settings.BobSpeedCrouch : Player.Settings.BobSpeedWalk)
            );

            // Reset the bob timer
            BobTimer = 0.0f;
        }
    }



}
