using UnityEngine;

public class ReloadCrank : Interactable , ICollectable
{
    public PickupData CrankPickupData;
    [field: SerializeField] public float CrankBoost { get; private set; } = 20f;

    public void Collect()
    {
        //pool manager handle collected 
        if(ObjectPickupUIHandler.Instance != null)
            ObjectPickupUIHandler.Instance.PickedUpObject(CrankPickupData);
        gameObject.SetActive(false);
    }
    
    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}
