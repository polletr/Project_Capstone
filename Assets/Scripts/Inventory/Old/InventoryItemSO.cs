using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSO/InventoryItems"), Serializable]
public class InventoryItemSO : ScriptableObject
{
    public string Name;

    public string Description;

    public int MaxStackSize;

    public bool QuickAccess;

    public Sprite Icon;
}
