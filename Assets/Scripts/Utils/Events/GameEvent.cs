using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    public UnityAction<Vector3, float> OnSoundEmitted;

    public UnityAction<IInventoryItem,Dictionary<InventoryItemData,int>> OnItemAdded;
    public UnityAction<IInventoryItem,Dictionary<InventoryItemData,int>> OnItemRemoved;
    public UnityAction<IInventoryItem,Dictionary<InventoryItemData,int>> OnItemEquipped;
}



