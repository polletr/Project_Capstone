using UnityEngine;

[RequireComponent(typeof(IndicatorHandler))]
public abstract class Interactable : MonoBehaviour
{
    public IndicatorHandler indicatorHandler { get; private set; }

    public GameEvent Event;

    public Transform TargetUIPos { get; set; }

    protected virtual void Awake()
    {
        TargetUIPos = transform;
        indicatorHandler = GetComponent<IndicatorHandler>();
    }
    public virtual void OnInteract()
    {
        if (TryGetComponent(out InteractTrigger trigger))
            trigger.onPlayerInteract.Invoke();

    }

    public virtual void OnLookAtInteractable()
    {

    }
}
