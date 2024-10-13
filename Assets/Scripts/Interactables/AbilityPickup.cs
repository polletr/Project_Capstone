using System;
using UnityEngine;

public class AbilityPickup : Interactable, ICollectable
{
    [field: SerializeField] public FlashlightAbility AbilityToPickup { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AbilityToPickup = GetComponentInChildren<FlashlightAbility>();
    }

    public void Collect()
    {
        gameObject.SetActive(false);
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}