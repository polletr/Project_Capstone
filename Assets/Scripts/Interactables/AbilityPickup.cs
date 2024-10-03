using UnityEngine;

public class AbilityPickup : MonoBehaviour, IInteractable, ICollectable
{
    public GameEvent Event;

    public FlashlightAbility AbilityToPickup;
    
    private InteractTrigger interactTrigger;

    public void Collect()
    {
        gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        if(TryGetComponent(out interactTrigger))
            interactTrigger.InvokeInteractTrigger();

        Event.OnInteractItem?.Invoke(this);
    }


}
