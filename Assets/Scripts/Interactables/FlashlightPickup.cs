using UnityEngine;
using UnityEngine.Events;

public class FlashlightPickup : MonoBehaviour, IInteractable, ICollectable
{
    public GameEvent Event;

    public UnityEvent OnCollect;

    public void Collect()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpFlashlight, transform.position);

        gameObject.SetActive(false);//Destroy(this.gameObject);// uhhhhhhh disable it? pool this shit 

        OnCollect.Invoke();
    }

    public void OnInteract()
    {
        Event.OnInteractItem?.Invoke(this);
    }
}
