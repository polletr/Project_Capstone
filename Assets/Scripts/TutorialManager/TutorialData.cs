using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TutorialMessage", menuName = "GameSO/TutorialMessage")]
public class TutorialData : ScriptableObject
{
    [field: SerializeField] public string Message { get; private set; }
    
    public UnityAction OnDoTutorial;
    public UnityAction OnEndTutorial;
    
    public void End()
    {
        OnEndTutorial?.Invoke();
        OnDoTutorial -= End;
    }
    
}