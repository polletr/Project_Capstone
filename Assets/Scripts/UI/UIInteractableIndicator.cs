using UnityEngine;
using UnityEngine.UI;

public class UIInteractableIndicator : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private UIAnimator eHolderAnimator;
    [SerializeField] private CanvasGroup circleImageCanvasGroup;
    [SerializeField] private Image lockedImage;

    private Vector3 targetPos;
    private Camera cam;

    private void Start()
    {
        if (lockedImage == null) { Debug.LogError("Missing reference: " + nameof(lockedImage)); }
        if (eHolderAnimator == null) { Debug.LogError("Missing reference: " + nameof(eHolderAnimator)); }
        if (circleImageCanvasGroup == null) { Debug.LogError("Missing reference: " + nameof(circleImageCanvasGroup)); }

        cam = Camera.main;

        SetLockedIndicator(false);
    }

    private void Update()
    {
        if (targetPos == null) { Debug.LogError("Missing Target set in IndicatorHandler"); return; }

        Vector3 screenPosition = cam.WorldToScreenPoint(targetPos);

        if (screenPosition.z > 0 && transform.position != screenPosition)
            transform.position = screenPosition;
    }

    public void SetCircleIndicator(float value)
    {
        if (eHolderAnimator == null || circleImageCanvasGroup == null) return;

        circleImageCanvasGroup.alpha = value;
    }

    public void TriggerTextIndicator(bool grow)
    {
        if (eHolderAnimator == null) return;
        eHolderAnimator.GrowInAnimate(grow);
    }

    public void SetIndicatorPosition(Vector3 position)
    {
        targetPos = position;
    }

    public void SetLockedIndicator(bool isLocked)
    {
        lockedImage.gameObject.SetActive(isLocked);
    }
}
