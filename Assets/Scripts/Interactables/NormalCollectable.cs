using UnityEngine;

public class NormalCollectable : Interactable , ICollectable
{
    public void Collect()
    {
        gameObject.SetActive(false);
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
        
        Collect();
    }
}
