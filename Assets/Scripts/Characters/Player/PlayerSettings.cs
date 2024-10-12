using System;
using UnityEngine;
[CreateAssetMenu(menuName = "GameSO/User Container", fileName = "PlayerPrefs"), Serializable]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement")]
    [Range(0.1f, 50f)] public float WalkingSpeed;
    [Range(0.1f, 50f)] public float RunningSpeed;
    [Range(0.1f, 50f)] public float CrouchingSpeed;

    [Header("Camera")]
    public float cameraSensitivityMouse = 1.0f;
    public float cameraSensitivityGamepad = 1.0f;
    public float cameraAcceleration = 5.0f;
    public float ClampAngleUp = -50.0f;
    public float ClampAngleDown = 40.0f;

    [Header("Flashlight Rotate")]
    public float FlashlightRotateSpeed = 100.0f;
    public float FlashlightAngleUp = 50.0f;
    public float FlashlightAngleDown = 10.0f;
    public float FlashlightReloadTime = 5.0f;

    [Header("Sound Distance")]
    [Range(0.5f, 5f)] public float WalkSoundRange = 2f;
    [Range(0.5f, 5f)] public float CrouchSoundRange = 0.5f;
    [Range(1f, 10f)] public float RunSoundRange = 5f;

    [Header("Health")]
    [Range(1, 5)] public int PlayerHealth = 3;
    public float HealthRegenRate = 1f;
    public float HealthRegenDelay = 2f;
    public float RespawnTime = 5f;

    [Header("Enemy Detection / Interaction")]
    public float MaxEnemyDistance = 5.0f;
    public float DropItemTime = 3f;
    public float InteractionRange = 2.0f;


    [Header("Bobbing")]
    public float BobAmount = 0.2f;
    public float BobSpeedWalk = 0.2f;
    public float BobSpeedRun = 0.2f;
    public float BobSpeedCrouch = 0.2f;
}
