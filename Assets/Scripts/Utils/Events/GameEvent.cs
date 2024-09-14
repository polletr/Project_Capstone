using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    public UnityAction<Vector3, float> OnSoundEmitted;

    public UnityAction<InventoryItemSO,Dictionary<InventoryItemSO,int>> OnItemAdded;
    public UnityAction<InventoryItemSO,Dictionary<InventoryItemSO,int>> OnItemRemoved;
    public UnityAction<InventoryItemSO,Dictionary<InventoryItemSO,int>> OnItemEquipped;
}



