using UnityEngine;

public class IndicatorHandler : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private float circleUIMinDistance = 2f;
    [SerializeField] private float circleUIMaxDistance = 10f;

    [Header("References")]
    [SerializeField] private SphereCollider triggerCol;

    [Header("Optional UI References")]
    [SerializeField] private ObjectFlare objectFlare;
    [field: SerializeField] public Transform TargetUIPos { get; set; }
    [field: SerializeField, Tooltip("Setup as child or hook up in inspector")]
    public UIInteractableIndicator IndicatorUI { get; private set; }

    private Camera cam;

    private Transform playerTarget;

    private void Start()
    {
        if (triggerCol == null)
        {
            Debug.LogError($"No SphereCollider found on {gameObject.name}");
            return;
        }
        triggerCol.isTrigger = true;
        triggerCol.radius = checkRadius;

        cam = Camera.main;
        var UI = GetComponentInChildren<UIInteractableIndicator>();
        IndicatorUI = UI ? UI : IndicatorUI;

        objectFlare = GetComponentInChildren<ObjectFlare>();

        IndicatorUI.SetIndicatorPosition(TargetUIPos != null ? TargetUIPos.position : transform.position);

        IndicatorUI.SetCircleIndicator(0);
        IndicatorUI.TriggerTextIndicator(false);
    }

    private void Update()
    {
        if (playerTarget == null || !playerTarget.gameObject.activeSelf)
        {
            IndicatorUI.SetCircleIndicator(0);
            IndicatorUI.TriggerTextIndicator(false);
            return;
        }

        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        
        var distance = Vector3.Distance(playerTarget.position, transform.position);

        IndicatorUI.SetCircleIndicator(distance < circleUIMaxDistance
            ? Mathf.InverseLerp(circleUIMaxDistance, circleUIMinDistance, distance)
            : 0);


        if(objectFlare == null) return;

        if (distance < circleUIMaxDistance)
            objectFlare.StopFlare();
        else
            objectFlare.StartFlare();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerTarget == null && other.TryGetComponent(out PlayerController player))
        {
            if (player.enabled)
                playerTarget = player.transform;
        }
    }
}