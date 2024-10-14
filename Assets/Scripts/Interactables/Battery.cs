using UnityEngine;

public class Battery : Interactable , ICollectable
{
    [field: SerializeField] public float BatteryCharge { get; private set; } = 20f;

    public void Collect()
    {
        //pool manager handle collected 
        gameObject.SetActive(false);
    }
    
    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}
