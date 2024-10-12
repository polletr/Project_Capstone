using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "GameEvent", menuName = "GameSO/GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    public UnityAction<Vector3, float> OnSoundEmitted;

    public UnityAction OnPlayerDeath;
    public UnityAction OnPlayerRespawn;

    public UnityAction OnPickupFlashlight;
    public UnityAction<FlashlightAbility> OnPickupAbility;
    public UnityAction<FlashlightAbility> OnRemoveAbility;

    public UnityAction<Door> OnTryToUnlockDoor;
    public UnityAction<ICollectable> OnInteractItem;
    
    public UnityAction<int> OnBatteryAdded;
    public UnityAction OnFinishRecharge;

    public UnityAction<LevelData> OnLevelChange;
    public UnityAction OnLoadStarterScene;

    public UnityAction<Transform> SetNewSpawn;
}


