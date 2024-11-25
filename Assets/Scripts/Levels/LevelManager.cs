using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public GameEvent Event;
    [field: SerializeField] public SceneNames MasterScene { get; private set; }
    [field: SerializeField] public SceneNames StartingScene { get; private set; }

    [SerializeField] private float _loadingWaitTime = 1f;

    private int _currentScene;
    private List<int> _currentLoadedScenes = new();
    private int masterSceneindex;
    

    private void OnEnable()
    {
        Event.OnLevelChange += StartLevelLoading;
        Event.OnReloadScenes += ReloadActiveScenes;
    }

    private void OnDisable()
    {
        Event.OnLevelChange -= StartLevelLoading;
        Event.OnReloadScenes -= ReloadActiveScenes;
    }

    private void Start()
    {
        masterSceneindex = Convert.ToInt32(MasterScene);
        _currentScene = Convert.ToInt32(StartingScene);
        LoadStartScene();
    }

    private void StartLevelLoading(LevelData level) => StartCoroutine(LoadLevels(level));

    private IEnumerator LoadLevels(LevelData level)
    {
        _currentScene = level.CurrentSceneName;
        //wait before loading the new scenes
        yield return new WaitForSeconds(_loadingWaitTime);

        //unload all scenes except the master scene , current scene and  the scenes to load
        foreach (var scene in GetActiveScenes())
        {
            if (scene != masterSceneindex && scene != _currentScene && !level.ScenesToLoad.Contains((SceneNames)scene))
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }

        //load the new scenes   
        foreach (var scene in level.ScenesToLoad)
        {
            if (!SceneManager.GetSceneByBuildIndex((int)scene).isLoaded) //check if the scene is already loaded
            {
                var sceneIndex = Convert.ToInt32(scene);
                SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            }
        }
    }

    /// <summary>
    /// Load a scene without any other scene
    /// use for final scene
    /// </summary>
    public void LoadOnlyScene(int sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load the first scene by adding the StartingScene to the MasterScene 
    /// </summary>
    private void LoadStartScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);
        asyncLoad.completed += (_) => { Event.OnLoadStarterScene?.Invoke(); };
    }


    private void ReloadActiveScenes()
    {
        StartCoroutine(UnloadScenesThenLoad());
    }

    private IEnumerator UnloadScenesThenLoad()
    {
        // Unload all active scenes except the MasterScene
        foreach (var scene in GetActiveScenes())
        {
            if (scene == masterSceneindex) continue;

            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(scene);

            // Wait for each scene to finish unloading
            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }

        // Once all scenes are unloaded, load the start scene
        LoadStartScene();
        // LoadLevels
    }
    

    #region Check Methods

    private List<int> GetActiveScenes()
    {
        _currentLoadedScenes.Clear();
        for (var i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            _currentLoadedScenes.Add(SceneManager.GetSceneAt(i).buildIndex);
        }
        return _currentLoadedScenes;
    }
    
    #endregion
    
}