using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private bool startingScene;
    [field: SerializeField] public string CurrentSceneName { get; private set; }
    [field: SerializeField] public List<string> ScenesToLoad { get; private set; } = new List<string>();

    [field: SerializeField] public Transform CheckPoint { get; private set; }

    public GameEvent Event;

    private void Start() 
    {
        if(startingScene)
        Event.OnLevelChange(this);
    }

    public void EnteredScene()
    {
        Event.OnLevelChange?.Invoke(this);

        if(CheckPoint != null)
        Event.SetNewSpawn?.Invoke(CheckPoint);
    }
}
