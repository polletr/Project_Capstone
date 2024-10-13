using Unity.VisualScripting;
using UnityEngine;

public class IndicatorHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float circleUIDistance = 10f;
    [SerializeField] private float textUIDistance = 2f;

    [Header("References")]
    [SerializeField] private UIInteractableIndicator indicatorUI;
    [SerializeField] private float radius = 5f;
    [SerializeField] private Transform target;

    private Camera cam; 

    private void Start()
    {
        cam = Camera.main;
        indicatorUI.SetCircleIndicator(0);
        indicatorUI.TriggerTextIndicator(false);
    }

    private void Update()
    {
        if (target == null)
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
                target = player.transform;
                break; 
            }
        }
    }

    private void UpdateIndicators()
    {
        float distance = Vector3.Distance(target.position, transform.position);

    
        indicatorUI.SetCircleIndicator(distance < circleUIDistance ? Mathf.InverseLerp(circleUIDistance, minDistance, distance) : 0);

       
        if (distance < textUIDistance)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, textUIDistance))            
            {
                if (hit.collider.TryGetComponent(out IndicatorHandler obj) && obj == this)
                    indicatorUI.TriggerTextIndicator(true);
                else
                    indicatorUI.TriggerTextIndicator(false);
            }
            else
                indicatorUI.TriggerTextIndicator(false);
        }
        else
            indicatorUI.TriggerTextIndicator(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
