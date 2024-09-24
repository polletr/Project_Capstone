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

    public override void StateFixedUpdate() { }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Ray ray = new Ray(player.Camera.position, player.Camera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, player.InteractionRange))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out IInteractable thing))
            {
                player.interactableObj = thing;
            }
        }
        else
        {
            player.interactableObj = null;
        }
    }

    public override void HandleChangeAbility(int d)
    {
        player.flashlight.ChangeSelectedAbility(d);
    }

    public override void HandleInteract()
    {
        if (player.interactableObj != null)
            player.ChangeState(new PlayerInteractState());
    }

}
