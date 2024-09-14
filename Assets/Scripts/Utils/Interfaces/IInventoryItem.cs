using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }

    bool QuickAccess { get; }

    Sprite Image { get; }

    void OnPickup();

    void OnDrop();
}
