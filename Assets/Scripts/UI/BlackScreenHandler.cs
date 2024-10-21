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
        canvasGroup = GetComponentInChildren<CanvasGroup>();
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
            Debug.Log("Inkoing fade in complete"); 
            OnFadeInComplete?.Invoke(); 
            Event.OnPlayerRespawn?.Invoke(); 
        });
    }

    public void FadeOut()
    {
            Event.OnReloadScenes?.Invoke(); 
        //fade out

        canvasGroup.DOFade(0, duration).SetEase(ease).OnComplete(() => { Debug.Log("Inkoing fade out complete"); OnFadeOutComplete?.Invoke(); });

    }

}
