using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : MonoBehaviour
{
    protected Vector3 _direction;

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
    }
    public virtual void HandleMovement(Vector2 dir) { }
    public virtual void HandleAttack()
    {
    }

    public virtual void HandleInteract()
    {
        player.ChangeState(new PlayerInteractState());

    }

    public virtual void StopInteract() { }

    public virtual void HandleDeath() { }

    private void Rotate()
    {
        player.gameObject.transform.rotation = Quaternion.Slerp(player.gameObject.transform.rotation, player.PlayerRotation, player.Settings.RotationSpeed);
    }

}
