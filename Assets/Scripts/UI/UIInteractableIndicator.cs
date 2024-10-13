using UnityEngine;

public class UIInteractableIndicator : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private UIAnimator animator;
    [SerializeField] private CanvasGroup canvasGroup;

    private Camera cam;

    private void Start()
    {
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
}
