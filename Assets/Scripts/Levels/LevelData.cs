using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelData : MonoBehaviour
{
    public int CurrentSceneName { get; private set; }

    [field: SerializeField] public List<SceneNames> ScenesToLoad { get; private set; } = new ();

    [field: SerializeField] public Transform CheckPoint { get; private set; }

    public GameEvent Event;

    private void Awake()
    {
        CurrentSceneName = gameObject.scene.buildIndex;
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

        if (CheckPoint != null)
            Event.SetNewSpawn?.Invoke(CheckPoint);
    }

    public void LoadSingleScene(SceneNames sceneName)
    {
        var sceneToLoad = Convert.ToInt32(sceneName);
        LevelManager.Instance.LoadOnlyScene(sceneToLoad);
    }
}