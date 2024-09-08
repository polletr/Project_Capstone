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
    public float MovementSpeed;
    [Range(0.5f, 5f)]
    public float WalkSoundRange = 2f;
    [Range(1f, 10f)]
    public float RunSoundRange = 5f;
    [Range(1, 5)]
    public int PlayerHealth = 3;



}
