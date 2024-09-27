using System;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItems"), Serializable]
public class InventoryItemSO : ScriptableObject
{
    public string Name;

    public string Description;

    public int MaxStackSize;

    public bool QuickAccess;

    public Sprite Icon;
}
