using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public Transform PlayerCheckpoint { get; private set; }

    public HubSO[] hubs;

    public void AddNextHub(int hubID)
    {
        HubSO hub = hubs[hubID - 1];
        Debug.Log("Loading hub: " + hub.SceneName);

        if (!IsSceneLoaded(hub.SceneName))
        {
            SceneManager.LoadScene(hub.SceneName, LoadSceneMode.Additive);
            RemoveHub(hub);
        }

    }

    public void RemoveHub(HubSO hub)
    {
        if (!string.IsNullOrEmpty(hub.PreviousSceneName) && IsSceneLoaded(hub.PreviousSceneName))
        {
            SceneManager.UnloadSceneAsync(hub.PreviousSceneName);
        }
    }

    public void SetCheckpoint(Hub hub)
    {
        PlayerCheckpoint = hub.Checkpoint;
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
    }
}
