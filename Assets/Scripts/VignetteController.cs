using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignetteController : MonoBehaviour
{
    private Volume postProcessVolume; // Reference to the Post Process Volume
    private Vignette vignette; // Vignette effect
    float oldIntensity;
    [SerializeField] float transitionTimer = 1.5f; // Speed of the effect

    void Start()
    {
        if (!TryGetComponent<Volume>(out postProcessVolume))
            return;

        // Try to get the Vignette effect from the Volume Profile
        if (postProcessVolume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = 0f; // Start with no vignette effect
        }
        else
        {
            Debug.LogWarning("Vignette effect not found in the Post Process Volume!");
        }
    }

    public void StartEffect(float targetIntensity)
    {
        if (oldIntensity != targetIntensity)
        {
            StopAllCoroutines();
            oldIntensity = targetIntensity;
            StartCoroutine(ChangeVignette(targetIntensity));
        }
    }

    private IEnumerator ChangeVignette(float targetIntensity)
    {
        if (vignette == null)
            yield break;

        float startIntensity = vignette.intensity.value;
        float timer = 0f;

        while (!Mathf.Approximately(vignette.intensity.value, targetIntensity))
        {
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, timer / transitionTimer);

            timer += Time.deltaTime;
            yield return null;
        }

        vignette.intensity.value = targetIntensity; // Ensure it reaches exact target
    }
}