using System;
using UnityEngine;

public class AbilityPickup : MonoBehaviour, IInteractable, ICollectable
{
    public GameEvent Event;

    [field: SerializeField] public FlashlightAbility AbilityToPickup { get; private set; }

    private InteractTrigger interactTrigger;

    private void Awake()
    {
        AbilityToPickup = GetComponentInChildren<FlashlightAbility>();
    }

    public void Collect()
    {
        gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        if (TryGetComponent(out interactTrigger))
            interactTrigger.InvokeInteractTrigger();

        Event.OnInteractItem?.Invoke(this);
    }
}