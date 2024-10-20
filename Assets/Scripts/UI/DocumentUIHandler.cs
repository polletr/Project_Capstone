using UnityEngine;
using UnityEngine.UI;

public class DocumentUIHandler : Singleton<DocumentUIHandler>
{
    private UIAnimator uiAnimator;
    private Image documentImage;

    public void Start()
    {
        documentImage = GetComponentInChildren<Image>();
        uiAnimator = GetComponentInChildren<UIAnimator>();
    }

    public void MoveDocumentUI(Sprite sprite)
    {
        documentImage.sprite = sprite;
        uiAnimator.MoveAnimate();
    }
}