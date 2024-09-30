using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "GameEvent", menuName = "GameSO/GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    public UnityAction<Vector3, float> OnSoundEmitted;

    public UnityAction<bool> OnFlashlightCollect;

    public UnityAction<Battery> OnCollectBattery;
    public UnityAction<Battery> OnChangeBattery;
    public UnityAction OnAskForBattery;

    public UnityAction<Hub> OnRoomInitialize;
    public UnityAction<HubSO> OnEnterRoom;
    public UnityAction<Transform> SetNewSpawn;
}



