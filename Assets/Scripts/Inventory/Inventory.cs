using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int slots = 3;

    private List<IInventoryItem> mItems = new List<IInventoryItem>();

    public GameEvent Event;

    public void AddItem(IInventoryItem item)
    {
        if (mItems.Count < slots)
        {
            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider.enabled)
            {
                collider.enabled = false;
                mItems.Add(item);

                item.OnPickup();

                if (Event.OnItemAdded != null)
                {
                    Event.OnItemAdded.Invoke(this, item);
                }
            }
        }
    }

    public void EquipItem(IInventoryItem item)
    {
        if (mItems.Contains(item))
        {
            if (Event.OnItemEquipped != null)
            {
                Event.OnItemEquipped.Invoke(this, item);
            }
        }
    }


    public void RemoveItem(IInventoryItem item)
    {
        if (mItems.Contains(item))
        {
            mItems.Remove(item);

            item.OnDrop();

            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider!=null)
            {
                collider.enabled = true;
            }

            if (Event.OnItemRemoved != null)
            {
                Event.OnItemRemoved.Invoke(this, item);
            }
        }
    }

}
