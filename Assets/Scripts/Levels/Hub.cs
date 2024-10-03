using UnityEngine;

public class Hub : MonoBehaviour
{
    public GameEvent Event;

    public HubSO hubSO;

    [field: SerializeField] public Transform Checkpoint { get; private set; }


    private void Start()
    {
        Debug.Log("Start Room");
        Event.OnRoomInitialize?.Invoke(this);
    }

    //Ontrigger event but not for last one 
    public void OnPlayerEnterHub()
    {
        Event.OnEnterRoom?.Invoke(hubSO);
    }
}

