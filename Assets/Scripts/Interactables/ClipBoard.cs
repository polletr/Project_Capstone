using UnityEngine;
using DG.Tweening;

public class ClipBoard : MonoBehaviour
{
    [field: SerializeField] public int Id { get; private set; }

    [Header("Animation")] [SerializeField] private float duration = 1f;
    [SerializeField] private Ease ease = Ease.Linear;

    private CanvasGroup textCanvasGroup;

    private void Start()
    {
        textCanvasGroup = GetComponentInChildren<CanvasGroup>();
        textCanvasGroup.alpha = 0;
    }

    public void InvokeID()
    {
        if (DormPuzzle.Instance)
            DormPuzzle.Instance.CheckPuzzle(Id);
    }

    public void ShowDeceasedText()
    {
        Debug.Log("Showing Deceased Text");
        textCanvasGroup.DOFade(1, duration).SetEase(ease);
    }
}