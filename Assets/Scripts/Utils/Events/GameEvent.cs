using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    public UnityAction<Vector3, float> OnSoundEmitted;

    public UnityAction<IInventoryItem,Dictionary<InventoryItemSO,int>> OnItemAdded;
    public UnityAction<IInventoryItem,Dictionary<InventoryItemSO,int>> OnItemRemoved;
    public UnityAction<IInventoryItem,Dictionary<InventoryItemSO, int>> OnItemEquipped;


    /*public UnityAction<Battery> OnCollectBattery;
    public UnityAction<bool,Battery> OnChangeBattery;*/
}



