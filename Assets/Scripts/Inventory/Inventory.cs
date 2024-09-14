using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int slots = 5;

    [SerializeField]
    private int qaSlots = 4;

    private List<IInventoryItem> mItems = new List<IInventoryItem>();

    private List<IInventoryItem> qaItems = new List<IInventoryItem>();

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

                if (qaItems.Count < qaSlots && item.QuickAccess)
                    qaItems.Add(item);

                item.OnPickup();

                if (Event.OnItemAdded != null)
                {
                    Event.OnItemAdded.Invoke(item);
                }
            }
        }
    }

    public void EquipItem(IInventoryItem item)
    {
        if (qaItems.Contains(item))
        {
            if (Event.OnItemEquipped != null)
            {
                Event.OnItemEquipped.Invoke(item);
            }
        }
    }

    public void QuickAccessItem(IInventoryItem item)
    {
        if (item.QuickAccess && qaItems.Count < qaSlots)
        {
            qaItems.Add(item);
        }
    }

    public void OnSelectItem()
    {
        
    }

    public void ChangeSelectedItem(IInventoryItem item, int direction)
    {
        int currentIndex = qaItems.IndexOf(item);
        (item as MonoBehaviour).gameObject.SetActive(false);
        // Update index based on scroll direction
        currentIndex += direction;

        // Loop through the inventory (circular switching)
        if (currentIndex >= qaItems.Count)
        {
            currentIndex = 0;
        }
        else if (currentIndex < 0)
        {
            currentIndex = qaItems.Count - 1;
        }

        // Equip the new item
        IInventoryItem newItem = qaItems[currentIndex];
        EquipItem(newItem);
    }


    public void RemoveItem(IInventoryItem item)
    {
        if (mItems.Contains(item))
        {
            mItems.Remove(item);

            if (qaItems.Contains(item))
                qaItems.Remove(item);

            item.OnDrop();

            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider!=null)
            {
                collider.enabled = true;
            }

            if (Event.OnItemRemoved != null)
            {
                Event.OnItemRemoved.Invoke(item);
            }
        }
    }

}
