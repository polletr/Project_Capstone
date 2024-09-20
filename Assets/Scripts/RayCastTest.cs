using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    public float maxDistance = 10f;    // Max range of the flashlight
    public float circleRadius = 2f;    // Radius of the circular beam at the max distance
    public int numRays = 12;           // Number of rays to form the circle (increase for smoother circle)

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // Cast the central ray (straight ahead)
        RaycastHit hit;
        if (Physics.Raycast(origin, forward, out hit, maxDistance))
        {
            Debug.Log("Center ray hit: " + hit.collider.gameObject.name);
        }

        // Cast surrounding rays to form the circular edge
        for (int i = 0; i < numRays; i++)
        {
            // Calculate angle for each ray around the circle (full 360 degrees)
            float angle = (i * 360f) / numRays;

            // Calculate the direction for each ray by rotating around the forward axis
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.right * circleRadius;

            // Add the forward direction to move the circle outward at maxDistance
            Vector3 rayTarget = (forward * maxDistance) + rayDirection;

            // Draw the ray from the flashlight position to the target point on the circle
            if (Physics.Raycast(origin, rayTarget.normalized, out hit, maxDistance))
            {
                Debug.Log("Circle ray hit: " + hit.collider.gameObject.name);
            }
        }
    }

    // Draw Gizmos to visualize the circular rays in the Scene view
    void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // Set the color of the gizmos (e.g., yellow for visualization)
        Gizmos.color = Color.yellow;

        // Draw the central ray gizmo
        Gizmos.DrawRay(origin, forward * maxDistance);

        // Draw the surrounding rays in a circle to visualize the flashlight beam
        for (int i = 0; i < numRays; i++)
        {
            // Calculate the angle for each ray in a circle
            float angle = (i * 360f) / numRays;

            // Calculate the direction for each ray in the circle
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.right * circleRadius;

            // Target point where the ray would hit on the circle
            Vector3 rayTarget = (forward * maxDistance) + rayDirection;

            // Draw the ray from the flashlight position to the target point
            Gizmos.DrawRay(origin, rayTarget.normalized * maxDistance);
        }
    }
}