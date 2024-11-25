using UnityEngine;
using UnityEngine.Events;

public class GlobalEventListener : MonoBehaviour
{
    public GlobalEventSO GlobalEvent;
    public UnityEvent OnTriggerGlobalEvent;
    public UnityEvent<Transform> OnTriggerSightEvent;

    private void OnEnable()
    {
        GlobalEvent.OnTriggerGlobalEvent += TriggerGlobalEvent;
        GlobalEvent.OnTriggerSightEvent += TriggerSightEvent;
    }

    private void OnDisable()
    {
        GlobalEvent.OnTriggerGlobalEvent -= TriggerGlobalEvent;
        GlobalEvent.OnTriggerSightEvent -= TriggerSightEvent;
    }

    private void TriggerGlobalEvent()
    {
        OnTriggerGlobalEvent.Invoke();
    }

    private void TriggerSightEvent(Transform transform)
    {
        OnTriggerSightEvent.Invoke(transform);
    }

}
