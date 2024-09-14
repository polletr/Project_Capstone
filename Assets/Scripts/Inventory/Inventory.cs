using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int slots = 5;

    [SerializeField]
    private int qaSlots = 4;

    [SerializeField] int loot = 1; // remember to change this into the loot amount we find 

    private List<InventoryItemSO> qaItems = new List<InventoryItemSO>();

    private Dictionary<InventoryItemSO, int> itemStackCount = new();

    public GameEvent Event;

    public void AddItem(IInventoryItem item)
    {
        int lootAmount = item.ItemSO.StackSize > 1 ? loot : 1; // remember to change this into the loot amount we find

        if (itemStackCount.ContainsKey(item.ItemSO))
        {
            int oldCount = StacksOfItem(item.ItemSO);
            itemStackCount[item.ItemSO] += lootAmount;
            int newCount = StacksOfItem(item.ItemSO);

            Debug.Log("Old count: " + oldCount + " New count: " + newCount);
            if (newCount > oldCount)
            {
                if (slots - SlotsTaken() >= 0)
                {
                    Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
                    if (collider.enabled)
                    {
                        collider.enabled = false;

                        item.ItemSO.OnPickup();

                        if (Event.OnItemAdded != null)
                        {
                            Event.OnItemAdded.Invoke(item.ItemSO, itemStackCount);
                        }
                    }
                }
                else
                {
                    itemStackCount[item.ItemSO] -= lootAmount;
                    Debug.Log("Inventory stack full but you have it");
                }
            }

        }
        else if (slots - SlotsTaken() >= 0)
        {
            Debug.Log("New Item picked up");
            Collider collider = item.gameObject.GetComponent<Collider>();
            if (collider.enabled)
            {
                collider.enabled = false;

                if (qaItems.Count < qaSlots && item.ItemSO.QuickAccess)
                    qaItems.Add(item.ItemSO);

                itemStackCount.Add(item.ItemSO, lootAmount);

                item.ItemSO.OnPickup();

                if (Event.OnItemAdded != null)
                {
                    Event.OnItemAdded.Invoke(item.ItemSO, itemStackCount);
                }
            }
        }
        else
        {
            Debug.Log("Inventory is full");
            //add no space event
        }

    }

    private int StacksOfItem(InventoryItemSO item) // this is the amount of stacks of the item in the inventory
    {
        return Mathf.CeilToInt(itemStackCount[item] / item.StackSize);
    }

    private int SlotsTaken() // this is the amount of slots taken by the items in the inventory
    {
        int numberofSlotsinDisctionary = 0;
        foreach (KeyValuePair<InventoryItemSO, int> item in itemStackCount)
        {
            numberofSlotsinDisctionary += StacksOfItem(item.Key);
        }

        return numberofSlotsinDisctionary;
    }

    public void EquipItem(IInventoryItem item)
    {
        if (qaItems.Contains(item.ItemSO))
        {
            if (Event.OnItemEquipped != null)
            {
                Event.OnItemEquipped.Invoke(item.ItemSO, itemStackCount);
            }
        }
    }

    public void QuickAccessItem(InventoryItemSO item)
    {
        if (item.QuickAccess && qaItems.Count < qaSlots)
        {
            qaItems.Add(item);
        }
    }

    public void OnSelectItem(int slotID)
    {

    }

    public void ChangeSelectedItem(IInventoryItem item, int direction)
    {
        int currentIndex = qaItems.IndexOf(item.ItemSO);
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
        if (itemStackCount.ContainsKey(item.ItemSO))
        {
            itemStackCount.Remove(item.ItemSO);

            if (qaItems.Contains(item.ItemSO))
                qaItems.Remove(item.ItemSO);

            item.ItemSO.OnDrop();

            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (Event.OnItemRemoved != null)
            {
                Event.OnItemRemoved.Invoke(item.ItemSO, itemStackCount);
            }
        }
    }

}
