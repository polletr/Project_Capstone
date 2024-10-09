using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    [SerializeField] private Ease ease = Ease.Linear;

    [SerializeField] private float duration = 1f;

    private Tween currentTween;
    private Vector2 startPos;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private bool isFadeChanged;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPos = rectTransform.anchoredPosition;
        isFadeChanged = canvasGroup.alpha != 0;
    }


    public void FadeAnimate()
    {

        canvasGroup.DOFade(1, duration).SetLoops(-1, LoopType.Yoyo);
            
        /*
        if (isFadeChanged)
            canvasGroup.DOFade(0, duration).SetEase(ease).OnComplete(() =>
            {
                isFadeChanged = !isFadeChanged;
                FadeAnimate();
            });
        else
             canvasGroup.DOFade(1, duration).SetEase(ease).SetLoops(-1).OnComplete(() =>
            {
                isFadeChanged = !isFadeChanged;
                FadeAnimate();
            });
            */

    }

    /*
    public void FadeInAnimate(bool fade)
    {
        if (fade)
            currentTween = canvasGroup.DOFade(1, duration).SetEase(ease);
        else
            currentTween = canvasGroup.DOFade(0, duration).SetEase(ease);

        currentTween.OnComplete(() =>
        {
            currentTween.Kill();
            isFadeChanged = fade;
        });
    }
    */


    public void RotateAnimate()
    {
        rectTransform.DORotate(new Vector3(0, 0, -180), duration).SetEase(ease).SetLoops(-1);
    }
}