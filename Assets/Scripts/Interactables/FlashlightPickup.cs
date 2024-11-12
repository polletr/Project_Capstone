using UnityEngine.Events;

public class FlashlightPickup : Interactable, ICollectable
{
    public PickupData FlashlightPickupData;
    public UnityEvent OnCollect;
    
    public void Collect()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpFlashlight, transform.position);
        gameObject.SetActive(false);//Destroy(this.gameObject);// uhhhhhhh disable it? pool this shit 

        OnCollect.Invoke();
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }

    public void ShowFlashlightPickupUI()
    {
        if (ObjectPickupUIHandler.Instance)
            ObjectPickupUIHandler.Instance.PickedUpObject(FlashlightPickupData);
    }
}
