using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public GameEvent Event;

    [field: SerializeField] public string MasterScene { get; private set; }
    [field: SerializeField] public string StartingScene { get; private set; }

    [SerializeField] private float _loadingWaitTime = 1f;

    private string _currentScene;
    private List<string> _currentLoadedScenes = new();

    private EventSystem _eventSystem;

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
        _eventSystem = FindObjectOfType<EventSystem>();
        _currentScene = StartingScene;
        LoadStartScene();
    }

    private void StartLevelLoading(LevelData level) => StartCoroutine(LoadLevels(level));

    private IEnumerator LoadLevels(LevelData level)
    {
        _currentScene = level.CurrentSceneName;
        //wait beofre loading the new scenes
        yield return new WaitForSeconds(_loadingWaitTime);

        //unload all scenes except the master scene , current scene and  the scenes to load
        foreach (var scene in GetActiveScenes())
        {
            if (scene != MasterScene && scene != _currentScene && !level.ScenesToLoad.Contains(scene))
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }

        //load the new scenes   
        foreach (var scene in level.ScenesToLoad)
        {
            if (!SceneManager.GetSceneByName(scene).isLoaded) //check if the scene is already loaded
            {
                SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
        }
    }

    public void LoadOnlyScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void LoadStartScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);
        asyncLoad.completed += (_) => Event.OnLoadStarterScene?.Invoke();
    }

    private List<string> GetActiveScenes()
    {
        _currentLoadedScenes.Clear();
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            _currentLoadedScenes.Add(SceneManager.GetSceneAt(i).name);
        }

        return _currentLoadedScenes;
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
            if (scene == MasterScene) continue;

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



    #region    Extra Methods

    /* private void LoadSceneWithDelay<T>(UnityAction<T> action, T parameter)
     {
         SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);
         action?.Invoke(parameter);
     }
     private void LoadSceneWithDelay(UnityAction action)
     {
         SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);
         action?.Invoke();
     }*/
    #endregion
}
