using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

[CreateAssetMenu(fileName = "SFXEvent", menuName = "SFXEvent", order = 0)]
public class SFXEvents : ScriptableObject
{
    [field: Header("Door SFX")]
    [field: SerializeField] public EventReference OpenDoor { get; private set; }
    [field: SerializeField] public EventReference CloseDoor { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PlayerSteps { get; private set; }
    [field: SerializeField] public EventReference HeavyToLowBreathing { get; private set; }
    [field: SerializeField] public EventReference PlayerHeartbeat { get; private set; }

    [field: Header("Environment Lights SFX")]
    [field: SerializeField] public EventReference LightConstantFlickering { get; private set; }



}
