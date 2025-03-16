using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Camera playerCamera;
    private Vector3 rotationOffset;

    void Start()
    {
        // Get the main camera
        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogWarning("No camera tagged as MainCamera found in the scene!");
        }

        // Define the rotation offset
        rotationOffset = new Vector3(-90f, 90f, 0f);
    }

    void Update()
    {
        if (playerCamera != null)
        {
            // Make the object look at the player camera
            transform.LookAt(playerCamera.transform);

            // Apply the rotation offset
            transform.rotation *= Quaternion.Euler(rotationOffset);
        }
    }
}