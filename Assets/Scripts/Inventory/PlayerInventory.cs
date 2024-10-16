using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [field: SerializeField] public float ChargesCollected { get; private set; }

    private Stack<FlashlightAbility> _collectedAbilitys = new();

    private int numAbilityCollectedperCheckpoint;
    private float numBatteryCollectedperCheckpoint;

    private List<int> _openDoorIDs = new();

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
        if (_openDoorIDs.Contains(door.OpenID))
        {
            door.OpenDoor();
            _openDoorIDs.Remove(door.OpenID);
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
                AddBattery(batteryItem.BatteryCharge);
                batteryItem.Collect();
                break;
            case AbilityPickup abilityPickup:
                _collectedAbilitys.Push(abilityPickup.AbilityToPickup);
                numAbilityCollectedperCheckpoint++;
                Event.OnPickupAbility?.Invoke(abilityPickup.AbilityToPickup);
                abilityPickup.Collect();
                break;
            case Key key:
                if(!_openDoorIDs.Contains(key.OpenID))
                _openDoorIDs.Add(key.OpenID);
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

    public void AddBattery(float charge)
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
        ChargesCollected += charge;
        numBatteryCollectedperCheckpoint += charge;
        Event.OnBatteryAdded?.Invoke(ChargesCollected);
    }

    public void RemoveBattery()
    {
        ChargesCollected = ChargesCollected - numBatteryCollectedperCheckpoint;
        Event.OnBatteryAdded?.Invoke(ChargesCollected);
    }
    

    public void RemoveAllKeys(LevelData data)
    {
        _openDoorIDs.Clear();
    }

    public bool HasKey(Door item)
    {
        return _openDoorIDs.Contains(item.OpenID);
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