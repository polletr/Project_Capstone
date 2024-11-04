using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [field: SerializeField] public float ChargesCollected { get; private set; }
    [field: SerializeField] public float CrankCollected { get; private set; }

    private readonly Stack<FlashlightAbility> collectedAbility = new();

    private int numAbilityCollectedperCheckpoint;
    private float numBatteryCollectedperCheckpoint;
    private float numCrankCollectedperCheckpoint;

    private List<int> _openDoorIDs = new();

    public GameEvent Event;
    
    private void OnEnable()
    {
        Event.OnInteractItem += CollectItem;
        Event.OnTryToUnlockDoor += TryToOpenDoor;
        Event.OnLevelChange += RemoveAllKeys;
        Event.OnPlayerDeath += PlayerDiedRemoveAbility;
        Event.OnPlayerDeath += RemoveBattery;
        Event.OnLevelChange += RestAbilityCheckpointCounter;
    }

    private void OnDisable()
    {
        Event.OnInteractItem -= CollectItem;
        Event.OnTryToUnlockDoor -= TryToOpenDoor;
        Event.OnLevelChange -= RemoveAllKeys;
        Event.OnPlayerDeath -= PlayerDiedRemoveAbility;
        Event.OnPlayerDeath -= RemoveBattery;
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

    private void CollectItem(ICollectable item)
    {
        switch (item)
        {
            case Battery batteryItem:
                AddBattery(batteryItem.BatteryCharge);
                batteryItem.Collect();
                break;
            case AbilityPickup abilityPickup:
                collectedAbility.Push(abilityPickup.AbilityToPickup);
                numAbilityCollectedperCheckpoint++;
                Event.OnPickupAbility?.Invoke(abilityPickup.AbilityToPickup);
                abilityPickup.Collect();
                break;
            case Key key:
                if (!_openDoorIDs.Contains(key.OpenID))
                    _openDoorIDs.Add(key.OpenID);
                Event.OnKeyPickup.Invoke(key.OpenID);
                item.Collect();
                break;
            case FlashlightPickup flashlightPickup:
                Event.OnPickupFlashlight?.Invoke();
                flashlightPickup.Collect();
                break; 
            case ReloadCrank crank:
                AddCrank(crank.CrankBoost);
                crank.Collect();
                break;
            default:
                Debug.Log($"Item not recognized wtf is this {item}");
                break;
        }

        // Display item in inventory
    }

    private void AddBattery(float charge)
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
        ChargesCollected += charge;
        numBatteryCollectedperCheckpoint += charge;
        Event.OnBatteryAdded?.Invoke(ChargesCollected);
    }
    private void AddCrank(float crank)
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
        CrankCollected += crank;
        numCrankCollectedperCheckpoint += crank;
        Event.OnCrankAdded?.Invoke(CrankCollected);
    }

    private void RemoveBattery()
    {
        ChargesCollected -= numBatteryCollectedperCheckpoint;
        Event.OnBatteryAdded?.Invoke(ChargesCollected);
    }


    private void RemoveAllKeys(LevelData data)
    {
        _openDoorIDs.Clear();
    }

    private void PlayerDiedRemoveAbility()
    {
        while (numAbilityCollectedperCheckpoint > 0 && collectedAbility.Count > 0)
        {
            var ability = collectedAbility.Pop();
            Event.OnRemoveAbility?.Invoke(ability);
            numAbilityCollectedperCheckpoint--;
        }

        RestAbilityCheckpointCounter(null);
    }

    private void RestAbilityCheckpointCounter(LevelData x) => numAbilityCollectedperCheckpoint = 0;
}