using UnityEngine;
using UnityEngine.Serialization;

public class Battery : Interactable , ICollectable
{
    public PickupData BatteryPickupData;
    [field: SerializeField] public float BatteryCharge { get; private set; } = 20f;

    public void Collect()
    {
        //pool manager handle collected 
        if(ObjectPickupUIHandler.Instance != null)
            ObjectPickupUIHandler.Instance.PickedUpObject(BatteryPickupData);
        gameObject.SetActive(false);
    }
    
    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}
