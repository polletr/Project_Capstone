using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class SubtitleManager : Singleton<SubtitleManager>
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

        float fadeInDuration = 0.5f;   // adjust as needed
        float fadeOutDuration = 0.5f;  // adjust as needed

        // --- Fade IN ---
        float t = 0f;
        float startAlpha = subtitleGroup.alpha; // in case it's partially visible
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            subtitleGroup.alpha = Mathf.Lerp(startAlpha, 1f, t / fadeInDuration);
            yield return null;
        }
        subtitleGroup.alpha = 1f; // ensure it's fully opaque

        // --- Wait full duration ---
        if (line.duration > 0)
        {
            yield return new WaitForSeconds(line.duration);

            // --- Fade OUT ---
            t = 0f;
            while (t < fadeOutDuration)
            {
                t += Time.deltaTime;
                subtitleGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
                yield return null;
            }
            subtitleGroup.alpha = 0f;

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