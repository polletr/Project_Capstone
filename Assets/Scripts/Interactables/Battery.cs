using UnityEngine;
public class Battery : MonoBehaviour, ICollectable, IInteractable
{
  public GameEvent Event;

    public void Collect()
    {
        //pool manager handle collected 
        gameObject.SetActive(false);
    }
    
    public void OnInteract()
    {
        Event.OnInteractItem?.Invoke(this);
    }
}
