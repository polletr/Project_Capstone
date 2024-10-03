using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    [SerializeField] float shakeIntensity = 0.1f; // How strong the shake is
    [SerializeField] float shakeDuration = 1f;    // How long the shake lasts
    [SerializeField] bool shakeX = true;          // Should the shake happen in the X axis?
    [SerializeField] bool shakeY = true;          // Should the shake happen in the Y axis?

    private Vector3 originalPosition;   // The object's original position
    private Coroutine shakeCoroutine;   // Reference to the shake coroutine

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
    }

    // Call this to start the shake
    public void ShakeObject()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);  // Stop any ongoing shake
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    // Coroutine to shake the object
    IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Calculate the remaining time proportion (from 1 at start to 0 at end)
            float timeFactor = 1f - (elapsedTime / shakeDuration);

            // Gradually decrease the shake strength using the time factor (ease-out effect)
            float currentIntensity = shakeIntensity * timeFactor;

            // Calculate random shake offsets with decreasing intensity
            float xOffset = shakeX ? Random.Range(-1f, 1f) * currentIntensity : 0f;
            float yOffset = shakeY ? Random.Range(-1f, 1f) * currentIntensity : 0f;

            // Apply shake to the object's position
            transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Reset the object's position after the shake is done
        transform.localPosition = originalPosition;
    }
}
