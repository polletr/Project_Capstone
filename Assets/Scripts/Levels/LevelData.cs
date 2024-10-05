using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public string CurrentSceneName { get; private set; }

    [field: SerializeField] public List<string> ScenesToLoad { get; private set; } = new List<string>();

    [field: SerializeField] public Transform CheckPoint { get; private set; }

    public GameEvent Event;

    private void Awake()
    {
        CurrentSceneName = gameObject.scene.name;
    }

    private void OnEnable()
    {
      Event.OnLoadStarterScene += EnteredScene;
    }

    private void OnDisable()
    {
        Event.OnLoadStarterScene -= EnteredScene;        
    }

    public void EnteredScene()
    {
        Event.OnLevelChange?.Invoke(this);

        if(CheckPoint != null)
        Event.SetNewSpawn?.Invoke(CheckPoint);
    }

}
