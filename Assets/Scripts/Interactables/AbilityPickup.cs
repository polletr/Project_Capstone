using System;
using Flashlight.Ability;
using UnityEngine;

public class AbilityPickup : Interactable, ICollectable
{
    [field: SerializeField] public FlashlightAbility AbilityToPickup { get; private set; }

    private PickupData abilityPickupData;
    
    protected override void Awake()
    {
        base.Awake();
        AbilityToPickup = GetComponentInChildren<FlashlightAbility>();
        abilityPickupData = AbilityToPickup.AbilityPickupData;
    }

    public void Collect()
    {
        if(ObjectPickupUIHandler.Instance != null)
            ObjectPickupUIHandler.Instance.PickedUpObject(abilityPickupData);
        gameObject.SetActive(false);
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}