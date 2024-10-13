using UnityEngine;

public class Key : Interactable , ICollectable
{
    [field: SerializeField] public Door doorToOpen { get; private set;}
    
    public void Collect()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
        gameObject.SetActive(false);
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}
