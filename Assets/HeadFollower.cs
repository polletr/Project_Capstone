using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadFollower : MonoBehaviour
{
    private Transform cameraTransform;

    [SerializeField] private Transform followTarget;

    [SerializeField] private MultiAimConstraint headAimConstraint;

    [SerializeField] private GameObject followObject;

    private Transform targetTransform;

    private void Start()
    {
        if (Camera.main != null) cameraTransform = Camera.main.transform;
        targetTransform = followTarget != null ? followTarget : cameraTransform;
    }

    private void FixedUpdate()
    {
        if (followObject.transform.position != targetTransform.position)
            followObject.transform.position = targetTransform.position;
    }
}