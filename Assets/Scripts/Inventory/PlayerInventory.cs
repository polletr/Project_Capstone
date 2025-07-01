using System.Collections.Generic;
using Flashlight.Ability;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [field: SerializeField] public float ChargesCollected { get; private set; }
    [field: SerializeField] public float CrankCollected { get; private set; }

    private readonly Stack<FlashlightAbility> collectedAbility = new();

    private int numAbilityCollectedperCheckpoint;
    private float numBatteryCollectedperCheckpoint;
    private float numCrankCollectedperCheckpoint;

    public List<int> _collectedKeyID = new();

    public GameEvent Event;
    
    private void OnEnable()
    {
        Event.OnInteractItem += CollectItem;
        Event.OnTryToUnlock += TryToOpenLock;
        Event.OnPlayerDeath += PlayerDiedRemoveAbility;
        Event.OnPlayerDeath += RemoveBattery;
        Event.OnLevelChange += RestAbilityCheckpointCounter;
    }

    private void OnDisable()
    {
        Event.OnInteractItem -= CollectItem;
        Event.OnTryToUnlock -= TryToOpenLock;
        Event.OnPlayerDeath -= PlayerDiedRemoveAbility;
        Event.OnPlayerDeath -= RemoveBattery;
        Event.OnLevelChange -= RestAbilityCheckpointCounter;
    }

    private void TryToOpenLock(IUnlockable locked)
    {
        Debug.Log(locked.OpenID);

        if (_collectedKeyID.Count > 0)
            Debug.Log(_collectedKeyID[0]);

        Debug.Log(_collectedKeyID.Contains(locked.OpenID));
        if (_collectedKeyID.Contains(locked.OpenID))
        {
            locked.Unlock();
            //Event.SetTutorialTextTimer.Invoke("Key has been used");
            _collectedKeyID.Remove(locked.OpenID);
        }
        else
        {
            locked.StayLocked();
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
                if (!_collectedKeyID.Contains(key.OpenID))
                {
                    _collectedKeyID.Add(key.OpenID);
                    Debug.Log("Item Collected" + key.OpenID);
                }
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
        _collectedKeyID.Clear();
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