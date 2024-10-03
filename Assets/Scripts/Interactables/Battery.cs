using UnityEngine;
public class Battery : MonoBehaviour, IDrainable<float>, ICollectable, IInteractable
{
    [SerializeField] private float batteryLife = 100f;

    public float BatteryLife
    {
        get { return batteryLife; }
        set { batteryLife = value; }
    }

    public GameEvent Event;

    public void Collect()
    {
        //pool manager handle collected 
        gameObject.SetActive(false);
    }
    public void Drain(float drainAmount)
    {
        BatteryLife -= drainAmount;
    }

    public bool IsBatteryDead()
    {
        return BatteryLife <= 0;
    }

    public void OnInteract()
    {
        Event.OnInteractItem?.Invoke(this);
    }
}
