using UnityEngine;

public class Key : Interactable , ICollectable
{
    public PickupData KeyPickupData;
    [field: SerializeField] public int OpenID { get; private set;}
    
    public void Collect()
    {
        if(ObjectPickupUIHandler.Instance)
            ObjectPickupUIHandler.Instance.PickedUpObject(KeyPickupData);
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
        gameObject.SetActive(false);
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}
