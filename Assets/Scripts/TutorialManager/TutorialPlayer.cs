using UnityEngine;

public class TutorialPlayer : MonoBehaviour 
{
    public TutorialData tutorialData1; 
    public TutorialData tutorialData2;
    
    
    public void InvokeEvent()
    {
        tutorialData1.OnDoTutorial?.Invoke();
    }
    public void InvokeEvent2()
    {
        tutorialData2.OnDoTutorial?.Invoke();
    }
}
