using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class SubtitleManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public CanvasGroup subtitleGroup;
    private Coroutine currentRoutine;

    public UnityEvent OnSubtitleEnd;

    public GameEvent gameEvent;
    void OnEnable()
    {
        if (gameEvent != null)
            gameEvent.OnRaised += ShowSubtitle;
    }

    void OnDisable()
    {
        if (gameEvent != null)
            gameEvent.OnRaised -= ShowSubtitle;
    }


    void Awake()
    {
        HideSubtitles();
    }

    public void ShowSubtitle(SubtitleLine line)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlaySubtitle(line));
    }

    private IEnumerator PlaySubtitle(SubtitleLine line)
    {
        subtitleText.text = line.text;
        subtitleText.color = line.textColor;
        subtitleGroup.alpha = 1;

        if (line.duration > 0)
        {
            yield return new WaitForSeconds(line.duration);
            HideSubtitles();
            OnSubtitleEnd?.Invoke();
        }
        // else wait for manual Continue
    }

    public void HideSubtitles()
    {
        subtitleGroup.alpha = 0;
        subtitleText.text = "";
    }

    public void Continue()
    {
        HideSubtitles();
        OnSubtitleEnd?.Invoke();
    }
}