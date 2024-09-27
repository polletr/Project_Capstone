using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Transform PlayerCheckpoint { get; private set; }

    public void AddNextHub(Hub hub)
    {
       SceneManager.LoadSceneAsync(hub.NextHubName, LoadSceneMode.Additive);
    }
    
    public void RemoveHub(Hub hub)
    {
        SceneManager.UnloadSceneAsync(hub.HubName);
    }
    
    public void SetCheckpoint(Hub hub)
    {
        PlayerCheckpoint = hub.CheckpointPos;
    }
  
}
