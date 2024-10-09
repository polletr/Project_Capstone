using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectFlare : MonoBehaviour
{

    [SerializeField] private float duration = 1f;
    [SerializeField] private float delay = 3f;
    [SerializeField] private Ease ease = Ease.Linear;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
    }

    private void Start()
    {
        
        Sequence sequenceScale = DOTween.Sequence();
        sequenceScale.Append(rectTransform.DOScale(new Vector3(0.5f,0.5f), duration).SetEase(ease))
            .AppendInterval(delay).
            Append(rectTransform.DOScale(new Vector3(1.5f, 1.5f), duration).SetEase(ease))
                .AppendInterval(0.5f).SetLoops(-1, LoopType.Yoyo);
        
        Sequence sequenceFade = DOTween.Sequence();
        sequenceFade.Append(canvasGroup.DOFade(0, duration).SetEase(ease))
            .AppendInterval(delay)
            .Append(canvasGroup.DOFade(1, duration).SetEase(ease))
                .AppendInterval(0.5f).SetLoops(-1, LoopType.Yoyo);
        
        
    }

    private void Update()
    {
        this.transform.LookAt(Camera.main.transform);
    }
}
