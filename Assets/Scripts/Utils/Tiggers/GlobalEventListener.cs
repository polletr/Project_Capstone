using UnityEngine;
using UnityEngine.Events;

public class GlobalEventListener : MonoBehaviour
{
    public GlobalEventSO GlobalEvent;
    public UnityEvent OnTriggerGlobalEvent;

    private void OnEnable()
    {
        GlobalEvent.OnTriggerGlobalEvent += TriggerGlobalEvent;
    }

    private void OnDisable()
    {
        GlobalEvent.OnTriggerGlobalEvent -= TriggerGlobalEvent;
    }

    private void TriggerGlobalEvent()
    {
        OnTriggerGlobalEvent.Invoke();
    }


}
