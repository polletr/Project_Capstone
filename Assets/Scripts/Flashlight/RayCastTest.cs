using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    public float maxDistance = 10f;    // Max range of the flashlight
    public float circleRadius = 2f;    // Radius of the circular beam at the max distance
    public int numRays = 12;           // Number of rays to form the circle

    private GameObject centerObject;
    private List<GameObject> currentlyCollidingObjects = new List<GameObject>(); // List of objects currently being hit
    private List<GameObject> objectsHitThisFrame = new List<GameObject>();       // Temporary list for this frame

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // Clear the list for this frame
        objectsHitThisFrame.Clear();

        // Cast the central ray (straight ahead)
        RaycastHit hit;
        if (Physics.Raycast(origin, forward, out hit, maxDistance))
        {
            centerObject = hit.collider.gameObject;
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

            // Perform the raycast
            if (Physics.Raycast(origin, rayTarget.normalized, out hit, maxDistance))
            {
                AddObjectToHitList(hit.collider.gameObject);
            }
        }

        // Check for objects that are no longer being hit by any rays
        RemoveNoLongerCollidingObjects();
    }

    // Add object to the list of hit objects if it's not already there
    void AddObjectToHitList(GameObject obj)
    {
        if (!objectsHitThisFrame.Contains(obj))
        {
            objectsHitThisFrame.Add(obj);
        }
    }

    // Remove objects from the current collision list if they are no longer being hit
    void RemoveNoLongerCollidingObjects()
    {
        // Loop through the list of objects that were colliding in the last frame
        for (int i = currentlyCollidingObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = currentlyCollidingObjects[i];

            // If the object was not hit by any rays this frame, remove it
            if (!objectsHitThisFrame.Contains(obj))
            {
                Debug.Log("Object is no longer being hit: " + obj.name);
                currentlyCollidingObjects.Remove(obj);
            }
        }

        // Add any new objects that were hit this frame
        foreach (GameObject obj in objectsHitThisFrame)
        {
            if (!currentlyCollidingObjects.Contains(obj))
            {
                currentlyCollidingObjects.Add(obj);
                Debug.Log("New object being hit: " + obj.name);
            }
        }
    }
}