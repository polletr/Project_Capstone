using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelData : MonoBehaviour
{
    public int CurrentSceneName { get; private set; }

    [field: SerializeField] public List<SceneNames> NextScenes { get; private set; } = new ();

    [field: SerializeField] public Transform CheckPoint { get; private set; }

    public GameEvent Event;

    private void Awake()
    {
        CurrentSceneName = gameObject.scene.buildIndex;
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