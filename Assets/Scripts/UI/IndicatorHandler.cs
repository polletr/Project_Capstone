using UnityEngine;

public class IndicatorHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float circleUIDistance = 10f;

    [Header("References")]
    [SerializeField] private float radius = 5f;
    [field: SerializeField] public Transform playerTarget { get; set; }
    [field: SerializeField] public Transform TargetUIPos { get; set; }
    [field: SerializeField] public UIInteractableIndicator IndicatorUI { get; private set;}

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        var UI = GetComponentInChildren<UIInteractableIndicator>();
        IndicatorUI = UI ? UI : IndicatorUI;
        
        IndicatorUI.SetIndicatorPosition(TargetUIPos? TargetUIPos.position : transform.position);

        IndicatorUI.SetCircleIndicator(0);
        IndicatorUI.TriggerTextIndicator(false);
    }

    private void Update()
    {
        if (playerTarget == null)
        {
            DetectTarget();
        }
        else
        {
            UpdateIndicators();
        }
    }

    private void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out PlayerController player))
            {
                playerTarget = player.transform;
                break;
            }
        }
    }

    private void UpdateIndicators()
    {
        float distance = Vector3.Distance(playerTarget.position, transform.position);

        IndicatorUI.SetCircleIndicator(distance < circleUIDistance ? Mathf.InverseLerp(circleUIDistance, minDistance, distance) : 0);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
