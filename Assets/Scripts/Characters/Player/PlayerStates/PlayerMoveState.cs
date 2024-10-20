using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{

    public PlayerMoveState(PlayerController playerController) : base(playerController) { }

    private Vector2 _dir = Vector2.zero;

    public override void EnterState()
    {
        playerAnimator.animator.Play(playerAnimator.IdleHash);
    }
    public override void ExitState()
    {
      
    }

    public override void StateFixedUpdate() { }

    public override void StateUpdate()
    {
        base.StateUpdate();

        HandleBobing();

        Ray ray = new Ray(player.PlayerCam.transform.position, player.PlayerCam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, player.Settings.MaxEnemyDistance))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out EnemyClass enemy))
            {
                player.AddEnemyToChaseList(enemy);
            }
        }

    }
    

    public override void HandleInteract()
    {
        if (player.interactableObj != null)
            player.ChangeState(player.InteractState);
    }

    public override void HandleRecharge()
    {
        player.flashlight.ZeroOutBattery();
        player.ChangeState(player.RechargeState);
    }
    
    private void HandleBobing()
    {
        // Check if the player is moving
        if (player.characterController.velocity.magnitude > 0.1f)
        {
            // Increment bobbing timer based on time and frequency
            _bobTimer += Time.deltaTime * (isRunning ? player.Settings.BobSpeedRun : isCrouching ? player.Settings.BobSpeedCrouch : player.Settings.BobSpeedWalk);

            // Calculate vertical bobbing offset using sine wave
            float bobOffsetY = Mathf.Sin(_bobTimer) * player.Settings.BobAmount; // Use BobAmount for vertical bobbing

            // Apply the calculated bobbing offset to the camera's local position
            player.PlayerCam.transform.localPosition = new Vector3(
                player.DefaultCameraLocalPosition.x,  // No left/right bobbing
                player.DefaultCameraLocalPosition.y + bobOffsetY,  // Up/down bobbing
                player.DefaultCameraLocalPosition.z
            );
        }
        else
        {
            // Smoothly return the camera to its default position when not moving
            player.PlayerCam.transform.localPosition = Vector3.Lerp(
                player.PlayerCam.transform.localPosition,
                player.DefaultCameraLocalPosition,
                Time.deltaTime * (isRunning ? player.Settings.BobSpeedRun : isCrouching ? player.Settings.BobSpeedCrouch : player.Settings.BobSpeedWalk)
            );

            // Reset the bob timer
            _bobTimer = 0.0f;
        }
    }



}
