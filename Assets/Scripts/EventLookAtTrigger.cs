using UnityEngine;
using UnityEngine.Events;

public class EventLookAtTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent LookAtTrigger;

    public void InvokeEvent()
    {
        LookAtTrigger.Invoke();
        Destroy(this.gameObject);
    }


}
