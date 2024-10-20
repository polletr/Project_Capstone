using UnityEngine;

public class GlobalTrigger : MonoBehaviour
{
    public GlobalEventSO GlobalEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GlobalEvent.OnTriggerGlobalEvent?.Invoke();
            Destroy(gameObject);
        }
    }

    public void OnTriggerGlobalEvent()
    {
        GlobalEvent.OnTriggerGlobalEvent?.Invoke();
    }
}
