using UnityEngine;
using UnityEngine.UI;

public class UIInteractableIndicator : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private UIAnimator animator;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image lockedImage;

    private Camera cam;

    private void Start()
    {
        if(lockedImage == null) { Debug.LogError("Missing reference: " + nameof(lockedImage)); }
        SetIndicatorSprite(false);
        cam = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(cam.transform.position);
    }

    public void SetCircleIndicator(float value)
    {
        if (animator == null || canvasGroup == null) return;

        canvasGroup.alpha = value;
    }

    public void TriggerTextIndicator(bool grow)
    {
        if (animator == null) return;
        animator.GrowInAnimate(grow);
    }

    public void SetIndicatorPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetIndicatorSprite(bool isLocked)
    {
        lockedImage.gameObject.SetActive(isLocked);
    }
}
