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

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, player.InteractionRange))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out IInteractable thing))
            {
                player.interactableObj = thing;
            }
        }
    }

    public override void HandleChangeAbility(int d)
    {
        player.flashlight.ChangeSelectedAbility(d);
    }

    public override void HandleInteract()
    {
        player.ChangeState(new PlayerInteractState());
    }

}
