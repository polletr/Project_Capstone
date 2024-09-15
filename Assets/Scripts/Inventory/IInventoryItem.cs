using UnityEngine;

public interface IInventoryItem
{
    InventoryItemData ItemSO { get; }
    void OnPickup();
    void OnDrop();
}
