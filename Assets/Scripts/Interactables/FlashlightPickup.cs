using UnityEngine;

public class FlashlightPickup : MonoBehaviour, IInteractable, ICollectable
{
    public GameEvent Event;

    public void Collect()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpFlashlight, transform.position);

        gameObject.SetActive(false);//Destroy(this.gameObject);// uhhhhhhh disable it? pool this shit 
    }

    public void OnInteract()
    {
        Event.OnInteractItem?.Invoke(this);
    }
}
