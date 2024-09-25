using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxBatteryCapacity; 
                    
    private Queue<Battery> _batteryPacks;

    private List<ICollectable> _collectables;


    public void CollectItem(ICollectable item)
    {
        if (TryGetComponent(out Battery battery))
        {
            AddBattery(battery);
        }
        else
        {
            _collectables.Add(item);
        }
    }

    public void AddBattery(Battery newBattery)
    {
        if(_batteryPacks.Count < maxBatteryCapacity)
        _batteryPacks.Enqueue(newBattery);
    }

    public bool CanGetBattery(out Battery newBattery)
    {
        if(_batteryPacks.Count > 0)
        {
            newBattery = _batteryPacks.Dequeue();
            return true;
        }

         newBattery = null;
         return false;
    }
}
