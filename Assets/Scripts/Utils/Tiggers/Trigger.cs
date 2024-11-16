using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent OnTriggerEvent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          TriggerEvent(); 
        }
    }
    
    public void TriggerEvent()
    {
        OnTriggerEvent.Invoke();
        Destroy(gameObject);
    }

}
