using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    public override void EnterState()
    {
        player.interactableObj.OnInteract();
        player.ChangeState(new PlayerMoveState());
    }
    public override void ExitState()
    {
        player.interactableObj = null;
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {

    }

    public override void HandleMovement(Vector2 dir)
    {

    }

    public override void HandleEquipItem(IInventoryItem item)
    {

    }

}
