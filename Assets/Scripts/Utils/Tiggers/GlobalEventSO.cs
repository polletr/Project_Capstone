using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "GameSO/GlobalEvent")]
public class GlobalEventSO : ScriptableObject
{
   public UnityAction OnTriggerGlobalEvent;
    public UnityAction<Transform> OnTriggerSightEvent;
}