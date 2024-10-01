using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "GameEvent", menuName = "GameSO/GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    public UnityAction<Vector3, float> OnSoundEmitted;

    public UnityAction OnPlayerDeath;

    public UnityAction<bool> OnFlashlightCollect;
    public UnityAction<FlashlightAbility> OnPickupAbility;

    public UnityAction<Door> OnTryToUnlockDoor;
    public UnityAction<ICollectable> OnInteractItem;
    public UnityAction<Battery> OnChangeBattery;
    public UnityAction OnAskForBattery;

    public UnityAction<Hub> OnRoomInitialize;
    public UnityAction<HubSO> OnEnterRoom;
    public UnityAction<Transform> SetNewSpawn;
}



