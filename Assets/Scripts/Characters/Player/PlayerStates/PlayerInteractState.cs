using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    public override void EnterState()
    {
        if (player.interactableItem != null)
        {
            // Add the item to the inventory
            player.inventory.AddItem(player.interactableItem);
            if (player.currentItemEquipped == null)
            {
                player.inventory.EquipItem(player.interactableItem);
            }
            // Clear the reference after interacting
            player.interactableItem = null;
        }
        else
        {
            // Handle other types of interaction (e.g., mini-games)
            Debug.Log("No item to interact with.");
        }
        player.ChangeState(new PlayerMoveState());
    }
    public override void ExitState()
    {

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

        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);
        goItem.GetComponent<Rigidbody>().isKinematic = true;

        goItem.transform.parent = player.MeleeSocketHand.transform;
        goItem.transform.localPosition = Vector3.zero;
        goItem.transform.localEulerAngles = Vector3.zero;

        player.currentItemEquipped = item;

    }

}
