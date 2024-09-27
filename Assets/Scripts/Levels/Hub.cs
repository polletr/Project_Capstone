using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub : MonoBehaviour
{
    public HubSO hubSO;
    [field: SerializeField] public Transform Checkpoint {get; private set;}
}
