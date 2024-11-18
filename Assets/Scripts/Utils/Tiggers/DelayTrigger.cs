using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class DelayTrigger : MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    
    public UnityEvent OnTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           TriggerEvent();
        }
    }
    
    public void TriggerEvent()
    {
       StartCoroutine(Delay());
    }
    
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        OnTrigger.Invoke();
        Destroy(gameObject);
    }
    
}
