using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "TutorialEvents", menuName = "GameSO/TutorialEvent", order = 0)]
public class TutorialEvents : ScriptableObject
{
    public UnityAction OnTurnOnFlashlight;
    public UnityAction OnTurnOffFlashlight;
    public UnityAction OnSwapAbility;
    public UnityAction OnReveal;
    public UnityAction OnDisappear;
    public UnityAction OnStun;
}
