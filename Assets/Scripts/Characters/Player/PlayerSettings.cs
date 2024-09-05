using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "User Containers/PlayerPrefs"), Serializable]
public class PlayerSettings : ScriptableObject
{
    [Range(0.0001f, 1f)]
    public float RotationSpeed;
    [Range(0.1f, 50f)]
    public float MovementSpeed;

}
