using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class BlackScreenCinematic : MonoBehaviour
{
     [SerializeField] private float duration;
    [SerializeField] private Ease ease = Ease.Linear;

    public UnityEvent OnFadeInComplete;
    public UnityEvent OnFadeOutComplete;

    private CanvasGroup canvasGroup;


    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }
    

    public void FadeIn()
    {
        //fade in
        canvasGroup.DOFade(1, duration).SetEase(ease).OnComplete(() =>
        {
            OnFadeInComplete?.Invoke();
        });
    }

    public void FadeOut()
    {
        canvasGroup.DOFade(0, duration).SetEase(ease).OnComplete(() =>
        {
            OnFadeOutComplete?.Invoke();
        });
    }

    public void FadeInAndOut()
    {
        canvasGroup.DOFade(1, duration).SetEase(ease).SetLoops(1,LoopType.Yoyo).OnComplete(() =>
        {
            Debug.Log("Faded wowwwwwww");
            OnFadeOutComplete?.Invoke();
        });
    }
}