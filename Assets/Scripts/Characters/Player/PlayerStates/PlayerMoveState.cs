using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{

    bool holdingDrop;

    float timer;
    public override void EnterState()
    {
        
    }
    public override void ExitState()
    {
        CancelDropItem();
    }

    public override void StateFixedUpdate() { }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (holdingDrop)
        {
            timer += Time.deltaTime;
            Debug.Log("DropTimer" +  timer);    
            if (timer > player.Settings.DropItemTime)
            {
                DropItem();
            }
        }
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

    public override void HandleChangeItem(int scrollDirection)
    {
        player.inventory.ChangeSelectedItem(player.currentItemEquipped, scrollDirection);
    }

    private void DropItem()
    {
        
        player.inventory.RemoveItem(player.currentItemEquipped);
        player.currentItemEquipped = null;
        CancelDropItem();
    }

    public override void HandleDropItem()
    {
        holdingDrop = true;
    }

    public override void CancelDropItem()
    {
        holdingDrop = false;
        timer = 0f;
    }
}
