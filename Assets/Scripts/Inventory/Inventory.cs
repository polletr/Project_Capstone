using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int slots = 5;

    [SerializeField]
    private int qaSlots = 4;

    [SerializeField] int loot = 1; // remember to change this into the loot amount we find 

    private List<IInventoryItem> qaItems = new List<IInventoryItem>();

    private Dictionary<InventoryItemData, int> itemStackCount = new();

    public GameEvent Event;

    public void AddItem(IInventoryItem item)
    {
        InventoryItemData itemData = item.ItemSO;
        int lootAmount = itemData.StackSize > 1 ? loot : 1; // remember to change this into the loot amount we find

        if (itemStackCount.ContainsKey(itemData))
        {
            int oldCount = StacksOfItem(itemData);
            itemStackCount[itemData] += lootAmount;
            int newCount = StacksOfItem(itemData);

            if (newCount > oldCount)
            {
                if (slots - SlotsTaken() >= 0)
                {
                    Debug.Log("Old Item");

                    Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
                    if (collider.enabled)
                    {
                        collider.enabled = false;

                        item.OnPickup();

                        if (Event.OnItemAdded != null)
                        {
                            Event.OnItemAdded.Invoke(item, itemStackCount);
                        }
                    }
                }
                else
                {
                    itemStackCount[itemData] -= lootAmount;
                    Debug.Log("Inventory stack full but you have it");
                }
            }
            else
            {
                Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
                if (collider.enabled)
                {
                    collider.enabled = false;

                    item.OnPickup();

                    if (Event.OnItemAdded != null)
                    {
                        Event.OnItemAdded.Invoke(item, itemStackCount);
                    }
                }
            }

        }
        else if (slots - SlotsTaken() > 0)
        {
            Debug.Log("New Item picked up");
            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider.enabled)
            {
                collider.enabled = false;

                QuickAccessItem(item);

                itemStackCount.Add(itemData, lootAmount);

                item.OnPickup();

                if (Event.OnItemAdded != null)
                {
                    Event.OnItemAdded.Invoke(item, itemStackCount);
                }
            }
        }
        else
        {
            Debug.Log("Inventory is full");
            //add no space event
        }

    }

    private int StacksOfItem(InventoryItemData item) // this is the amount of stacks of the item in the inventory
    {
        return Mathf.CeilToInt(itemStackCount[item] / item.StackSize);
    }

    private int SlotsTaken() // this is the amount of slots taken by the items in the inventory
    {
        int numberofSlotsinDisctionary = 0;
        foreach (KeyValuePair<InventoryItemData, int> item in itemStackCount)
        {
            numberofSlotsinDisctionary += StacksOfItem(item.Key);
        }

        return numberofSlotsinDisctionary;
    }

    public void EquipItem(IInventoryItem item)
    {
        if (qaItems.Contains(item))
        {
            if (Event.OnItemEquipped != null)
            {
                Event.OnItemEquipped.Invoke(item, itemStackCount);
            }
        }
    }

    public void QuickAccessItem(IInventoryItem item)
    {
        if (item.ItemSO.QuickAccess && qaItems.Count < qaSlots)
        {
            qaItems.Add(item);
        }
    }

    public void OnSelectItem(int slotID)
    {

    }

    public void ChangeSelectedItem(IInventoryItem item, int direction)
    {
        int currentIndex = 0;

        if (item != null)
        {
            currentIndex = qaItems.IndexOf(item);
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
        }

        if (qaItems.Count > 0)
        {
            // Equip the new item
            IInventoryItem newItem = qaItems[currentIndex];
            EquipItem(newItem);
        }
    }


    public void RemoveItem(IInventoryItem item)
    {
        if (item != null)
        {
            if (itemStackCount.Count > 0 && itemStackCount.ContainsKey(item.ItemSO))
            {
                if (itemStackCount[item.ItemSO] > 1)
                {
                    itemStackCount[item.ItemSO]--;
                }
                else
                {
                    itemStackCount.Remove(item.ItemSO);
                }

                if (qaItems.Contains(item))
                    qaItems.Remove(item);

                item.OnDrop();

                Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = true;
                }

                if (Event.OnItemRemoved != null)
                {
                    Event.OnItemRemoved.Invoke(item, itemStackCount);
                }
            }
        }
    }

}
