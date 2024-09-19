using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "User Containers/PlayerPrefs"), Serializable]
public class PlayerSettings : ScriptableObject
{
    [Range(0.001f, 0.2f)]
    public float RotationSpeed;
    [Range(0.1f, 50f)]
    public float WalkingSpeed;
    [Range(0.1f, 50f)]
    public float RunningSpeed;
    [Range(0.1f, 50f)]
    public float CrouchingSpeed;
    [Range(0.1f, 50f)]
    public float AimingSpeed;
    [Range(0.5f, 5f)]
    public float WalkSoundRange = 2f;
    [Range(0.5f, 5f)]
    public float CrouchSoundRange = 0.5f;
    [Range(1f, 10f)]
    public float RunSoundRange = 5f;
    [Range(1, 5)]
    public int PlayerHealth = 3;
    public float DropItemTime = 3f;

    public float cameraSensitivityMouse = 1.0f;
    public float cameraSensitivityGamepad = 1.0f;
    public float cameraAcceleration = 5.0f;


}
