using UnityEngine;

[RequireComponent(typeof(GlobalEventListener), typeof(BoxCollider))]
public class CheckPointTrigger : MonoBehaviour
{
    public GameEvent Event;
    public Transform CheckPoint;

    private BoxCollider checkCol;
    private PlayerController player;
    private GlobalEventListener globalTrigger;

    private void Awake()
    {
        globalTrigger = GetComponent<GlobalEventListener>();
        checkCol = GetComponent<BoxCollider>();
        checkCol.isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out player))
        {
            player.SetSpawn(CheckPoint);
            Event.OnTriggerCheckpoint?.Invoke(globalTrigger.GlobalEvent);
            checkCol.enabled = false;
        }
    }
}