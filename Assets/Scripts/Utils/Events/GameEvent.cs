using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "GameEvent", menuName = "GameSO/GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    public UnityAction<Vector3, float> OnSoundEmitted;

    public UnityAction OnPlayerDeath;
    public UnityAction OnFadeBlackScreen;
    public UnityAction OnPlayerRespawn;

    public UnityAction OnPickupFlashlight;
    public UnityAction<FlashlightAbility> OnPickupAbility;
    public UnityAction<FlashlightAbility> OnRemoveAbility;

    public UnityAction<Door> OnTryToUnlockDoor;
    public UnityAction<ICollectable> OnInteractItem;

    public UnityAction<float> OnBatteryAdded;
    public UnityAction<float> OnCrankAdded;
    public UnityAction OnFinishRecharge;

    
    /// <summary>
    /// Level Events
    /// </summary>
    public UnityAction<LevelData> OnLevelChange;
    public UnityAction OnReloadScenes;
    public UnityAction OnLoadStarterScene;
    public UnityAction<GlobalEventSO> OnTriggerCheckpoint;
    public UnityAction OnLoadCheckPointEvents;

    public UnityAction<Transform> SetNewSpawn;

    public UnityAction<string> SetTutorialText;
    public UnityAction<string> SetTutorialTextTimer;

    public UnityAction<PlayerController> PlayerExitedSafeZone;
    public UnityAction<int> PlayerEnteredSafeZone;

    int timer = 0;

    public void HandlePlayerFootSteps(Vector3 position, float range)
    {
        if (timer == 0)
        {
            OnSoundEmitted?.Invoke(position, range);
        }

        timer++;
        timer %= 5;

    }
}


