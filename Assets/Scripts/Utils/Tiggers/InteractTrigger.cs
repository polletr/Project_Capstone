using UnityEngine;
using UnityEngine.Events;

public class InteractTrigger : MonoBehaviour
{
    public UnityEvent onPlayerInteract;

    public void InvokeInteractTrigger()
    {
        onPlayerInteract?.Invoke();
    }
}