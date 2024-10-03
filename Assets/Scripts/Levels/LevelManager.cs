using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.SearchService;

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
        SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);
    }

    private void StartLevelLoading(LevelData level) => StartCoroutine(LoadLevels(level));

    private IEnumerator LoadLevels(LevelData level)
    {
        _currentScene = level.CurrentSceneName;
        //wait beofre loading the new scenes
        WaitForSeconds waiter = new WaitForSeconds(_loadingWaitTime);
        Debug.Log($"Loading Level.....{waiter}");
        yield return waiter;

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
