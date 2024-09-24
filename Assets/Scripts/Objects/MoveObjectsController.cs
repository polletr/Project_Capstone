using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectsController : MonoBehaviour
{
    private Vector3 originalPosition; // Store the original position of the object

    [SerializeField] private Vector3 direction;
    [SerializeField] private float duration;

    private void Awake()
    {
        originalPosition = transform.position; // Save the original position
    }

    // Move the object in the specified direction
    public void MoveObject()
    {
        StartCoroutine(SmoothMove(originalPosition + direction, duration));
    }

    // Snap the object to the new position
    private void SnapToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition; // Set the position directly
    }

    // Smoothly move the object to the target position
    private IEnumerator SmoothMove(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition; // Ensure the final position is set
    }
}
