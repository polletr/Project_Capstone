using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxBatteryCapacity;

    [field: SerializeField] public int ExtraBatteryCount { get; private set; }

    private Stack<FlashlightAbility> _collectedAbilitys = new();

    private int numAbilityCollectedperCheckpoint;
    private int numBatteryCollectedperCheckpoint;

    private Dictionary<Door, ICollectable> keys = new();

    public GameEvent Event;


    private void OnEnable()
    {
        Event.OnInteractItem += CollectItem;
        Event.OnTryToUnlockDoor += TryToOpenDoor;
        Event.OnLevelChange += RemoveAllKeys;
        Event.OnPlayerDeath += PlayerDiedRemoveAbility;
        Event.OnLevelChange += RestAbilityCheckpointCounter;
    }

    private void OnDisable()
    {
        Event.OnInteractItem -= CollectItem;
        Event.OnTryToUnlockDoor -= TryToOpenDoor;
        Event.OnLevelChange -= RemoveAllKeys;
        Event.OnPlayerDeath -= PlayerDiedRemoveAbility;
        Event.OnLevelChange -= RestAbilityCheckpointCounter;
    }

    private void TryToOpenDoor(Door door)
    {
        if (HasKey(door))
        {
            door.UnlockDoor();
            keys.Remove(door);
        }
        else
        {
            door.LockedDoor();
        }
    }

    public void CollectItem(ICollectable item)
    {
        switch (item)
        {
            case Battery batteryItem:
                AddBattery();
                break;
            case AbilityPickup abilityPickup:
                _collectedAbilitys.Push(abilityPickup.AbilityToPickup);
                numAbilityCollectedperCheckpoint++;
                Event.OnPickupAbility?.Invoke(abilityPickup.AbilityToPickup);
                abilityPickup.Collect();
                break;
            case Key key:
                Debug.Log("PICKED UP ITEM");
                keys.Add(key.doorToOpen, key);
                item.Collect();
                break;
            case FlashlightPickup flashlightPickup:
                Event.OnPickupFlashlight?.Invoke();
                flashlightPickup.Collect();
                break;
            default:
                Debug.Log($"Item not recognized wtf is this {item}");
                break;
        }

        // Display item in inventory
    }

    public void AddBattery()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
        ExtraBatteryCount++;
        numBatteryCollectedperCheckpoint++;
        Event.OnBatteryAdded?.Invoke(ExtraBatteryCount);
    }
    

    public void RemoveAllKeys(LevelData data)
    {
        keys.Clear();
    }

    public bool HasKey(Door item) // musse wtf change this later
    {
        return keys.ContainsKey(item);
    }

    private void PlayerDiedRemoveAbility()
    {
        while (numAbilityCollectedperCheckpoint > 0 && _collectedAbilitys.Count > 0)
        {
            FlashlightAbility ability = _collectedAbilitys.Pop();
            Event.OnRemoveAbility?.Invoke(ability);
            numAbilityCollectedperCheckpoint--;
        }
    }

    private void RestAbilityCheckpointCounter(LevelData x) => numAbilityCollectedperCheckpoint = 0;
}