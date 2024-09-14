using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "InventoryItem", order = 0)]
public class InventoryItemSO : ScriptableObject  
{
    public string Name;
    public string Description;
    public int SlotSize;
    public int StackSize; 
    public bool QuickAccess;
    public Sprite Image;
   public void OnPickup() { }
   public void OnDrop() { }
}
