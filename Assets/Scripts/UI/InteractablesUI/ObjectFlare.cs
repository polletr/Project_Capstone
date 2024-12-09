using UnityEngine;
using DG.Tweening;

public class ObjectFlare : MonoBehaviour
{

    [SerializeField] private float duration = 1f;
    [SerializeField] private float delay = 3f;
    [SerializeField] private Ease ease = Ease.Linear;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Camera cam;

    private Sequence scaleSequence;
    private Sequence fadeSequence;

    private void Awake()
    {
        cam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
    }

    private void Start()
    {
        scaleSequence = DOTween.Sequence();
        scaleSequence.Append(rectTransform.DOScale(new Vector3(0.2f,0.2f), duration).SetEase(ease))
            .AppendInterval(delay)
            .Append(rectTransform.DOScale(new Vector3(0.1f, 0.1f), duration))
            .AppendInterval(0.5f).SetLoops(-1, LoopType.Yoyo);
        
        fadeSequence = DOTween.Sequence();
        fadeSequence.Append(canvasGroup.DOFade(0, duration).SetEase(ease))
            .AppendInterval(delay)
            .Append(canvasGroup.DOFade(1, duration))
            .AppendInterval(0.5f).SetLoops(-1, LoopType.Yoyo);

    }

    public void StartFlare()
    {
        scaleSequence.Play();
        fadeSequence.Play();
    }

    public void StopFlare()
    {
        scaleSequence.Pause();
        fadeSequence.Pause();
        canvasGroup.alpha = 0;
    }


    private void Update()
    {
        transform.LookAt(cam.transform);
    }
}
