using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxBatteryCapacity;

    [SerializeField] private Battery battery;
    
    private Queue<Battery> _batteryPacks = new();

    private List<ICollectable> _collectables;

    public GameEvent Event;


    private void Awake()
    { 
      battery = Instantiate(new GameObject().AddComponent<Battery>(), transform);  
      AddBattery(battery);
      SendBattery();
    }

    private void OnEnable()
    {
        Event.OnCollectBattery += AddBattery;
        Event.OnAskForBattery += SendBattery;
    }

    private void OnDisable()
    {
        Event.OnCollectBattery -= AddBattery;
        Event.OnAskForBattery -= SendBattery;
    }

    private void Update()
    {
        Debug.Log("Battery: " + _batteryPacks.Count);
    }

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
        //display item in inventory
    }

    public void AddBattery(Battery newBattery)
    {
        if (_batteryPacks.Count < maxBatteryCapacity)
        {
            newBattery.gameObject.SetActive(false);
            _batteryPacks.Enqueue(newBattery);
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
}
