using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxBatteryCapacity;

    [SerializeField] private Battery battery;
    
    private Queue<Battery> _batteryPacks = new();

    public int BatteryCount => _batteryPacks.Count;

    private Dictionary<Door,ICollectable> keys = new();

    public GameEvent Event;


    private void Awake()
    { 
      battery = Instantiate(new GameObject().AddComponent<Battery>(), transform);  
      AddBattery(battery);
      SendBattery();
    }

    private void OnEnable()
    {
        Event.OnInteractItem += CollectItem;
        Event.OnAskForBattery += SendBattery;
        Event.OnTryToUnlockDoor += TryToOpenDoor;
    }

    private void OnDisable()
    {
        Event.OnInteractItem -= CollectItem;
        Event.OnAskForBattery -= SendBattery;
        Event.OnTryToUnlockDoor -= TryToOpenDoor;

    }

    private void TryToOpenDoor(Door door)
    {
        if (HasKey(door))
        {
            door.UnlockDoor();  
        }
        else
        {
            door.LockedDoor();
        }
    }

    public void CollectItem(ICollectable item)
    {
        if (item is Battery batteryItem)
        {
            AddBattery(batteryItem);
        }
        else if (item is AbilityPickup abilityPickup)
        {
            Event.OnPickupAbility?.Invoke(abilityPickup.AbilityToPickup);
            abilityPickup.Collect();
        }
        else if (item is Key key)
        {
            Debug.Log("PICKED UP ITEM");
            keys.Add(key.doorToOpen, key);
            item.Collect();
        }

        // Display item in inventory
    }

    public void AddBattery(Battery newBattery)
    {
        if (_batteryPacks.Count < maxBatteryCapacity)
        {
            _batteryPacks.Enqueue(newBattery);
            newBattery.Collect();
        }
        else
        {
            Debug.Log("Battery inventory is full");
        }

        Debug.Log("Battery added to inventory");
    }

    public bool CanGetBattery(out Battery newBattery)
    {
        if (_batteryPacks.Count > 0)
        {
            newBattery = _batteryPacks.Dequeue();
            Debug.Log("Battery sent from inventory");
            return true;
        }

        newBattery = null;
        return false;
    }

    public void SendBattery()
    {
        if (CanGetBattery(out Battery newBattery))
        {
            Event.OnChangeBattery?.Invoke(newBattery);
        }
        else
        {
            Debug.Log("No battery in inventory");
        }
    }

    public bool HasKey(Door item) // musse wtf change this later
    {
        return keys.ContainsKey(item);
    }

}
