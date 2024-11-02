using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveObjectsController : MonoBehaviour
{
    private Vector3 targetPos;

    [SerializeField] private Vector3 direction;
    [SerializeField] private float duration;

    private bool isMoving;
    [SerializeField] private bool dragSound;

    [SerializeField] UnityEvent OnStartMoving;
    [SerializeField] UnityEvent OnStopMoving;

    private void Awake()
    {
        targetPos = transform.position; // Save the original position
    }

    // Move the object in the specified direction
    public void MoveObject()
    {
        OnStartMoving.Invoke();
        if (dragSound)
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.DragObjects, transform.position);

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
        OnStopMoving.Invoke();

        isMoving = false;
    }
}
