using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{

    public SubtitleLine line;
    
    public void TriggerSubtitle()
    {
        SubtitleManager.Instance.ShowSubtitle(line);
    }



}
