using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Inventory Components")]
    [SerializeField] private int quantity; 
    [field: SerializeField] public InventoryItem CurrentItem { get; set;}  

    public void OnDrop(PointerEventData eventData)
    {
       var draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();

        if (draggedItem == null) return;


        // Check if the slot is empty (no child item exists)
        if (CurrentItem == null && transform.childCount == 0)
        {
            // Assign the dragged item to this slot
            draggedItem.SetSlotParent(transform);
            quantity = draggedItem.Quantity;

            // Update the slot UI to reflect the item and its quantity
            draggedItem.SetUpUI(draggedItem.ItemSO, draggedItem.Quantity);
        }
        else
        {
            // If slot is not empty, check for stackable items
            var currentSlotItem = GetComponentInChildren<InventoryItem>();
            if (currentSlotItem != null && currentSlotItem.ItemSO == draggedItem.ItemSO && draggedItem.ItemSO.MaxStackSize > 1)
            {
                // Calculate the new total quantity
                int totalQuantity = currentSlotItem.Quantity + draggedItem.Quantity;
                int maxStackSize = draggedItem.ItemSO.MaxStackSize;

                // Stack items up to the max stack size
                if (totalQuantity <= maxStackSize)
                {
                    currentSlotItem.Quantity = totalQuantity;
                    currentSlotItem.SetUpUI(currentSlotItem.ItemSO, totalQuantity);
                    Destroy(draggedItem.gameObject);  // Destroy the dragged item after stacking
                }
                else
                {
                    int excess = totalQuantity - maxStackSize;
                    currentSlotItem.Quantity = maxStackSize;
                    currentSlotItem.SetUpUI(currentSlotItem.ItemSO, maxStackSize);

                    draggedItem.Quantity = excess;
                    draggedItem.SetUpUI(draggedItem.ItemSO, excess);
                    draggedItem.SetSlotParent(draggedItem.parentAfterDrag);  
                }
            }
        }
    }

  /*  public void AddItemToSlot(InventoryItem item, int amount)
    {
        if (currentItem == null)
        {
            PlaceItem(item, amount);
        }
        else if (currentItem == item && item.ItemSO.StackSize > 1)
        {
            StackItem(item, amount);
        }
    }

    private void StackItem(InventoryItem item, int amount)
    {
        int totalAmount = quantity + amount;
        int maxStackSize = item.ItemSO.StackSize;

        if (totalAmount <= maxStackSize)
        {
            quantity = totalAmount;
            currentItem.SetUpUI(item.ItemSO, totalAmount);
        }
        else
        {
            int excess = totalAmount - maxStackSize;
            quantity = maxStackSize;
            currentItem.SetUpUI(item.ItemSO, maxStackSize);
            CreateExcessItem(item.ItemSO, excess);
        }
    }

    private void CreateExcessItem(InventoryItemData item, int excess)
    {


    }

        private void PlaceItem(InventoryItem item, int amount)
    {
        currentItem = item;
        quantity = amount;
        currentItem.SetUpUI(item.ItemSO, amount);
    }

    public void RemoveItemFromSlot()
    {
        currentItem = null;
        quantity = 0;
    }*/
}
