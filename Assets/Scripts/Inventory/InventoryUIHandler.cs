using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUIHandler : MonoBehaviour
{
    [SerializeField]
    private List<InventoryButton> inventoryButtons = new();

    [SerializeField]
    private List<InventoryQuickAccess> quickAccessInventorySlot;

    public GameEvent Event;

    private void OnEnable()
    {
        Event.OnItemAdded += PopulateUI;
        Event.OnItemRemoved += PopulateUI;
    }

    private void OnDisable()
    {
        Event.OnItemAdded -= PopulateUI;
        Event.OnItemRemoved -= PopulateUI;

    }

    public void PopulateUI(IInventoryItem item, Dictionary<InventoryItemData, int> itemDictionary)
    {

        foreach (InventoryButton button in inventoryButtons)
        {
            button.Item = null;
            button.Icon.sprite = null;
            button.AmountText.text = "";
        }

        int i = 0;
        foreach (KeyValuePair<InventoryItemData, int> obj in itemDictionary)
        {
            int totalAmount = obj.Value;
            int stackSize = obj.Key.StackSize;

            // While there are still items to distribute across slots
            while (totalAmount > 0)
            {
                // Calculate how much to put in the current slot
                int amountToPut = Mathf.Min(totalAmount, stackSize);

                // Assign the item to the current inventory button
                inventoryButtons[i].Item = obj.Key;

                // Set the amount text to the amount in this slot
                if (amountToPut > 1)
                    inventoryButtons[i].AmountText.text = amountToPut.ToString();
                else
                    inventoryButtons[i].AmountText.text = "";

                // Set the icon for this item
                inventoryButtons[i].Icon.sprite = obj.Key.Image;


                // Reduce the total amount by the amount we just put in the slot
                totalAmount -= amountToPut;

                // Move to the next slot for any remaining items
                i++;
            }
        }

    }

    private void PopulateQuickAccess(List<IInventoryItem> items)
    {
        int i = 0;



        quickAccessInventorySlot.ForEach(slot =>
        {
            slot.Item = null;
            slot.Icon.sprite = null;
            slot.AmountText.text = "";
        });

        foreach (IInventoryItem item in items)
        {

            //item.ItemSO.
        }

    }


    // add description to the inventory popup 
}

