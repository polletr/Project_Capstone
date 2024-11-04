using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollower : MonoBehaviour
{
    private Transform cameraTransform;

    [SerializeField] private Transform followTarget;
    [SerializeField] private Transform headTransform; // Reference to the NPC's head transform
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation
    [SerializeField] private float maxHeadRotationX = 45f; // Maximum upward/downward rotation limit
    [SerializeField] private float maxHeadRotationY = 45f; // Maximum left/right rotation limit

    private Coroutine headFollowCoroutine;

    private void Start()
    {
        if (Camera.main != null) cameraTransform = Camera.main.transform;
        StartFollowing(); // Start following the target or camera
    }

    public void StartFollowing()
    {
        if (headFollowCoroutine != null)
        {
            StopCoroutine(headFollowCoroutine); // Stop any existing coroutine
        }
        headFollowCoroutine = StartCoroutine(FollowCoroutine());
    }

    private IEnumerator FollowCoroutine()
    {
        while (true) // This will run indefinitely until stopped
        {
            FollowTarget(); // Call the method to follow the target or camera
            yield return null; // Wait for the next frame
        }
    }

    private void FollowTarget()
    {
        Transform target = followTarget != null ? followTarget : cameraTransform;

        if (target == null) return;

        // Calculate the world space direction to the target
        Vector3 directionToTarget = target.position - headTransform.position;

        // Calculate the target rotation in world space
        Quaternion targetWorldRotation = Quaternion.LookRotation(directionToTarget);

        // Convert the target rotation to local space relative to the enemy's transform
        Quaternion targetLocalRotation = Quaternion.Inverse(transform.rotation) * targetWorldRotation;

        // Clamp the rotation angles in local space
        Vector3 clampedRotation = ClampRotation(targetLocalRotation.eulerAngles);

        // Apply the clamped rotation to the head smoothly
        headTransform.localRotation = Quaternion.Slerp(
            headTransform.localRotation,
            Quaternion.Euler(targetLocalRotation.eulerAngles),
            rotationSpeed * Time.deltaTime
        );
    }

    private Vector3 ClampRotation(Vector3 eulerAngles)
    {
        // Clamp the X rotation
        float clampedX = Mathf.Clamp(eulerAngles.x, -maxHeadRotationX, maxHeadRotationX);

        // Clamp the Y rotation
        float clampedY = Mathf.Clamp(eulerAngles.y, -maxHeadRotationY, maxHeadRotationY);

        // Keep Z rotation unchanged
        float clampedZ = eulerAngles.z;

        return new Vector3(clampedX, clampedY, clampedZ);
    }

    public void StopFollowing()
    {
        if (headFollowCoroutine != null)
        {
            StopCoroutine(headFollowCoroutine);
            headFollowCoroutine = null; // Clear the reference
        }
    }

    public void SetFollowTarget(Transform newTarget)
    {
        followTarget = newTarget;
        StartFollowing(); // Restart the following coroutine
    }
}
