using UnityEngine;
using UnityEngine.UI;

public class DocumentUIHandler : Singleton<DocumentUIHandler>
{
    [SerializeField] private UIAnimator uiAnimator;
    [SerializeField] private UIAnimator closeUIText;
    private Image documentImage;

    public void Start()
    {
        documentImage = GetComponentInChildren<Image>();
    }

    public void MoveDocumentUI(Sprite sprite, bool isOpen)
    {
        documentImage.sprite = sprite;
        uiAnimator.MoveInAnimate(isOpen);
        closeUIText.FadeInAnimate(isOpen);
    }
}