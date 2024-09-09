using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public override void EnterState()
    {
        player.animator.Play(IdleHash);

    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {
    }

    public override void StateUpdate()
    {
        Rotate();
        player.characterController.SimpleMove(_direction.normalized * player.Settings.MovementSpeed);
        
        if (_direction != null && player.Settings != null)
        {
            if (_direction.sqrMagnitude > 0f)
            {
                player.Event.OnSoundEmitted.Invoke(player.transform.position, player.Settings.WalkSoundRange);
            }
        }

    }

    public override void HandleMovement(Vector2 dir)
    {
        _direction = new Vector3(dir.x, 0, dir.y);
    }

    private void Rotate()
    {
        player.gameObject.transform.rotation = Quaternion.Slerp(player.gameObject.transform.rotation, player.PlayerRotation, player.Settings.RotationSpeed);
    }


}
