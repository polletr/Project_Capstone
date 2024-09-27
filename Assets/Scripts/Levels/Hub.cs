using UnityEngine;

public class Hub : MonoBehaviour
{
    [field: SerializeField] public string HubName { get; private set; }
    [field: SerializeField] public string NextHubName { get; private set; }
    [field: SerializeField] public Transform CheckpointPos { get; private set; }
}
