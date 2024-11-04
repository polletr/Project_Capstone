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
    // Public method to start following the player
    private void Start()
    {
        StartFollowing(); // Start following the target or camera
    }

    // Public method to start following the target or camera
    public void StartFollowing()
    {
        if (headFollowCoroutine != null)
        {
            StopCoroutine(headFollowCoroutine); // Stop any existing coroutine
        }
        headFollowCoroutine = StartCoroutine(FollowCoroutine());
    }

    // Coroutine to rotate the NPC's head towards the target or camera
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
        Transform target;
        if (followTarget != null)
        {
            target = followTarget;
        }
        else
        {
            cameraTransform = Camera.main.transform;
            target = cameraTransform;

        }

        // Get the direction to the target
        Vector3 directionToTarget = target.position - headTransform.position;

        // Calculate the rotation to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        // Get the clamped rotation angles
        //Vector3 clampedRotation = ClampRotation(targetRotation.eulerAngles);

        // Apply the clamped rotation to the head
        headTransform.localRotation = Quaternion.Slerp(headTransform.rotation, Quaternion.Euler(targetRotation.eulerAngles), rotationSpeed * Time.deltaTime);
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

    // Optional: Public method to stop following
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
