using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public GameEvent Event;

    [field: SerializeField] public string MasterScene { get; private set; }
    [field: SerializeField] public string StartingScene { get; private set; }

    [SerializeField] private float _loadingWaitTime = 1f;

    private string _currentScene;
    private List<string> _currentLoadedScenes = new();

    private void OnEnable()
    {
        Event.OnLevelChange += StartLevelLoading;
    }

    private void OnDisable()
    {
        Event.OnLevelChange -= StartLevelLoading;
    }

    private void Start()
    {
        _currentScene = StartingScene;
        StartCoroutine(LoadSceneWithDelay(Event.OnLoadStarterScene));
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

    private IEnumerator LoadSceneWithDelay(UnityAction action)
    {
        SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);
        yield return new WaitForSeconds(_loadingWaitTime);
        action?.Invoke();  // Invoking the action with no parameters
    }
    private IEnumerator LoadSceneWithDelay<T>(UnityAction<T> action, T parameter)
    {
        SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);
        yield return new WaitForSeconds(_loadingWaitTime);
        action?.Invoke(parameter);
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
}
