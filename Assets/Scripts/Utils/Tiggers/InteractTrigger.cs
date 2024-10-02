using UnityEngine;
using UnityEngine.Events;

public class InteractTrigger : MonoBehaviour
{
    public UnityEvent onPlayerInteract;

    IInteractable interactable;
    private void Awake()
    {
       TryGetComponent(out interactable);
    }

    public void OnInteract()
    {
        onPlayerInteract?.Invoke();
    }
}