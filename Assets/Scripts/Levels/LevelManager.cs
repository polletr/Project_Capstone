using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameEvent Event;

    [Header("Master Scene"), TextArea(1, 2)]
    public string MasterSceneName;

    [Header("Starter Hub/Scene")]
    public HubSO StarterHubSO;

    [field: SerializeField]
    public Transform PlayerCheckpoint { get; private set; }

    private HubSO _previousHub;
    private Dictionary<HubSO, Hub> hubs = new Dictionary<HubSO, Hub>();

    private void Start()
    {
        //hubs[StarterHubSO].gameObject.SetActive(true);// enable starter hub (tutorial) if it has a hub 

        //if we need to set first checkpoint do in inspector

        SceneManager.LoadSceneAsync(StarterHubSO.ChallengeSceneName, LoadSceneMode.Additive); // load starter scene
        GetHub(StarterHubSO.NextHub).gameObject.SetActive(true);                              //disable previous hub
        _previousHub = StarterHubSO;
    }

    private void OnEnable()
    {
        Event.OnRoomInitialize += RegisterHubs;
        Event.OnEnterRoom += EnteredHub;
    }

    private void OnDisable()
    {
        Event.OnRoomInitialize -= RegisterHubs;
        Event.OnEnterRoom -= EnteredHub;
    }

    private void RegisterHubs(Hub hub)
    {
        hubs.Add(hub.hubSO, hub);
        hub.gameObject.SetActive(false);
    }

    public void EnteredHub(HubSO hubSO)
    {
       StartCoroutine(WaitForLoadingScene());

        if (hubSO.NextHub != null && GetHub(hubSO))
            GetHub(hubSO.NextHub).gameObject.SetActive(true); //enable next hub 

        UnloadScene(_previousHub.ChallengeSceneName);         //unloading previous challenge

        if (hubSO.ChallengeSceneName != "")
            SceneManager.LoadSceneAsync(hubSO.ChallengeSceneName, LoadSceneMode.Additive);  //load new challenge34

        if (_previousHub != StarterHubSO)
            GetHub(_previousHub).gameObject.SetActive(false);  //disable previous hub

        _previousHub = hubSO;                                  //set previous hub to current hub

        SetCheckpoint(GetHub(hubSO));                          //set checkpoint

    }

    private IEnumerator WaitForLoadingScene()
    {
       
       yield return new WaitForSecondsRealtime(1f);



        yield return null;

    }

    private void UnloadScene(string sceneName)
    {
        if (IsSceneLoaded(sceneName))
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        else
            Debug.LogWarning("Scene not loaded: " + _previousHub.ChallengeSceneName + "wtf");
    }

    private void UnloadChallengeScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != MasterSceneName && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    public void SetCheckpoint(Hub hub)
    {
        PlayerCheckpoint = hub.Checkpoint;
    }

    private bool IsSceneLoaded(string sceneName) // this should never be needed there must be a starter scene loaded
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

    private Hub GetHub(HubSO hubSO)
    {
        return hubs[hubSO];
    }
}
