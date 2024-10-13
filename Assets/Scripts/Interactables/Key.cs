using UnityEngine;

public class Key : MonoBehaviour ,IInteractable , ICollectable
{
    public GameEvent Event;
    [field: SerializeField] public Door doorToOpen { get; private set;}
    
    public void Collect()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
        gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        Event.OnInteractItem?.Invoke(this);
        if (TryGetComponent(out InteractTrigger trigger))
        {
            trigger.onPlayerInteract.Invoke();
        }

    }
}
