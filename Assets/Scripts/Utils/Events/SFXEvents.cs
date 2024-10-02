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
    [field: SerializeField] public EventReference EasyLockedDoor { get; private set; }
    [field: SerializeField] public EventReference TenseLockedDoor { get; private set; }
    [field: SerializeField] public EventReference UnlockDoor { get; private set; }


    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PlayerSteps { get; private set; }
    [field: SerializeField] public EventReference HeavyToLowBreathing { get; private set; }
    [field: SerializeField] public EventReference PlayerHeartbeat { get; private set; }
    [field: SerializeField] public EventReference PickUpBatteries { get; private set; }

    [field: Header("Flashlight")]
    [field: SerializeField] public EventReference PickUpFlashlight { get; private set; }
    [field: SerializeField] public EventReference FlashlightOnOff { get; private set; }
    [field: SerializeField] public EventReference FlashlightReload { get; private set; }

    [field: Header("Environment Lights SFX")]
    [field: SerializeField] public EventReference LightConstantFlickering { get; private set; }
    [field: SerializeField] public EventReference LightTurnOn { get; private set; }
    [field: SerializeField] public EventReference LightFlickerOnce { get; private set; }

    [field: Header("Enemies")]
    [field: SerializeField] public EventReference Cry { get; private set; }

}
