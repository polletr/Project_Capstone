using UnityEngine;

public interface IInventoryItem
{
    InventoryItemSO ItemSO { get; }
    void OnPickup();
    void OnDrop();
}
