using UnityEngine;

public class Key : MonoBehaviour ,IInteractable , ICollectable
{
    public GameEvent Event;
    [field: SerializeField] public Door doorToOpen { get; private set;}
    
    public void Collect()
    {
       gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        Event.OnInteractItem?.Invoke(this);
    }
}
