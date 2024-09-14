using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUIHandler : MonoBehaviour
{
    [SerializeField]
    private List<InventoryButton> inventoryButtons = new();

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
            button.item = null;
            button.icon.sprite = null;
            button.amountText.text = "";
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
                inventoryButtons[i].item = obj.Key;

                // Set the amount text to the amount in this slot
                if (amountToPut > 1)
                    inventoryButtons[i].amountText.text = amountToPut.ToString();
                else
                    inventoryButtons[i].amountText.text = "";

                // Set the icon for this item
                inventoryButtons[i].icon.sprite = obj.Key.Image;

                // Reduce the total amount by the amount we just put in the slot
                totalAmount -= amountToPut;

                // Move to the next slot for any remaining items
                i++;
            }
        }

/*        int i = 0;
        foreach (KeyValuePair<InventoryItemData, int> obj in itemDictionary)
        {
            if (obj.Key.StackSize >= obj.Value)
            {
                inventoryButtons[i].item = obj.Key;

                if (obj.Value > 1)
                    inventoryButtons[i].amountText.text = obj.Value.ToString();
                else
                    inventoryButtons[i].amountText.text = "";

                inventoryButtons[i].icon.sprite = obj.Key.Image;

            }
            else
            {
                for (int j = i; j < (Mathf.CeilToInt(obj.Value / obj.Key.StackSize)); j++)
                {
                    inventoryButtons[j].item = obj.Key;
                    if (obj.Value > 1)
                        inventoryButtons[j].amountText.text = obj.Value.ToString();
                    else
                        inventoryButtons[j].amountText.text = "";
                    inventoryButtons[j].icon.sprite = obj.Key.Image;
                    i = j;

                }
            }
            i++;

        }
*/    }

    // add description to the inventory popup 
}

