using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class BlackScreenHandler : MonoBehaviour
{
    public GameEvent Event;

    [SerializeField] private float duration;
    [SerializeField] private Ease ease = Ease.Linear;

    public UnityEvent OnFadeInComplete;
    public UnityEvent OnFadeOutComplete;

    private CanvasGroup canvasGroup;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        Event.OnFadeBlackScreen += FadeIn;
    }

    private void OnDisable()
    {
        Event.OnFadeBlackScreen -= FadeIn;
    }

    private void FadeIn()
    {
        //fade in

        canvasGroup.DOFade(1, duration).SetEase(ease).OnComplete(() =>
        {
            Event.OnReloadScenes?.Invoke();
            Event.OnPlayerRespawn?.Invoke();
            OnFadeInComplete?.Invoke();
            canvasGroup.alpha = 0;
        });

    }

    public void FadeOut()
    {
        canvasGroup.alpha = 1;
        //fade out
        canvasGroup.DOFade(0, duration).SetEase(ease).OnComplete(() =>
        {
            OnFadeOutComplete?.Invoke(); 
        });

    }

}
