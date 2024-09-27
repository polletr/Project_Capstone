using UnityEngine;

[CreateAssetMenu(fileName = "Hub", menuName = "Hub")]
public class HubSO : ScriptableObject
{
    public string SceneName;
    public string NextSceneName;
    public string PreviousSceneName;
}
