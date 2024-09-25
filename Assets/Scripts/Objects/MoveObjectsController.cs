using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectsController : MonoBehaviour
{
    private Vector3 targetPos;

    [SerializeField] private Vector3 direction;
    [SerializeField] private float duration;

    [SerializeField] private bool isMoving;

    private void Awake()
    {
        targetPos = transform.position; // Save the original position
    }

    // Move the object in the specified direction
    public void MoveObject()
    {
        targetPos += direction;

        if (!isMoving)
        {
            StartCoroutine(SmoothMove(duration));
        }
    }


    // Smoothly move the object to the target position
    private IEnumerator SmoothMove(float duration)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.position = targetPos; // Ensure the final position is set
        isMoving = false;
    }
}
