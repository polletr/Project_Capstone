using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }

    string Description { get; }

    int SlotSize { get; }

    int StackSize { get; }

    bool QuickAccess { get; }

    Sprite Image { get; }


    void OnPickup();
    void OnDrop();
}
