using UnityEngine;

[CreateAssetMenu(fileName = "HubNum", menuName = "GameSO/HubSO")]
public class HubSO : ScriptableObject
{
    [TextArea(1,2)]
    public string ChallengeSceneName;
    [Space(1)]
    public HubSO NextHub;
}
