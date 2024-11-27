using UnityEngine;

public class LevelCheckPointManager : MonoBehaviour
{
    public GameEvent Event;
    private GlobalEventSO checkpointEvent;

    private void OnEnable()
    {
        Event.OnTriggerCheckpoint += SetCheckpointEvent;
        Event.OnLoadCheckPointEvents += LoadCheckpointEvents;
      //  Event.OnLevelChange += RestCheckpointEvent;
    }
    
    private void OnDisable()
    {
        Event.OnTriggerCheckpoint -= SetCheckpointEvent;
        Event.OnLoadCheckPointEvents -= LoadCheckpointEvents;
    }
    
    private void SetCheckpointEvent(GlobalEventSO newGlobalEvent)
    {
        checkpointEvent = newGlobalEvent;
    }
    
    /*private void RestCheckpointEvent(LevelData level)
    {
        checkpointEvent = null;
    }*/

    private void LoadCheckpointEvents()
    {
        if (checkpointEvent == null) return;


        Debug.Log("calling Global Event");
        checkpointEvent.OnTriggerGlobalEvent?.Invoke();
    }
}
