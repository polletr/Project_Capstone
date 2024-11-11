using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    private Volume currentVolume; // The current post-processing volume in the scene
    [SerializeField] private VolumeProfile targetProfile; // The new profile to transition to

    private VolumeProfile originalProfile; // To store the original profile for potential reset

    private void Start()
    {
        currentVolume = GetComponent<Volume>();
        if (currentVolume == null || targetProfile == null)
        {
            Debug.LogWarning("Please assign both the current volume and the target profile.");
            return;
        }

        // Save the original profile
        originalProfile = currentVolume.profile;
    }

    public void StartFullProfileTransition(float transitionDuration)
    {
        StartCoroutine(LerpAllEffectsToNewProfile(targetProfile, transitionDuration));
    }

    private IEnumerator LerpAllEffectsToNewProfile(VolumeProfile newProfile, float duration)
    {
        float timeElapsed = 0f;

        // Dictionary to hold initial and target values for each parameter in each effect
        Dictionary<VolumeComponent, Dictionary<string, (object startValue, object targetValue)>> effectParameters = new Dictionary<VolumeComponent, Dictionary<string, (object, object)>>();

        // Loop through all components in the current profile and their matching components in the target profile
        foreach (var currentComponent in currentVolume.profile.components)
        {
            // Find the component in the target profile that matches the type of the current component
            var targetComponent = newProfile.components.Find(comp => comp.GetType() == currentComponent.GetType());

            if (targetComponent != null)
            {
                // Initialize a dictionary to store properties for the component
                var parameters = new Dictionary<string, (object, object)>();

                // Use reflection to access all properties of each effect
                foreach (var field in currentComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (field.FieldType == typeof(ClampedFloatParameter) || field.FieldType == typeof(FloatParameter))
                    {
                        var currentParameter = (FloatParameter)field.GetValue(currentComponent);
                        var targetParameter = (FloatParameter)field.GetValue(targetComponent);

                        if (currentParameter != null && targetParameter != null)
                        {
                            parameters[field.Name] = (currentParameter.value, targetParameter.value);
                        }
                    }
                    else if (field.FieldType == typeof(ColorParameter))
                    {
                        var currentParameter = (ColorParameter)field.GetValue(currentComponent);
                        var targetParameter = (ColorParameter)field.GetValue(targetComponent);

                        if (currentParameter != null && targetParameter != null)
                        {
                            parameters[field.Name] = (currentParameter.value, targetParameter.value);
                        }
                    }
                }

                effectParameters[currentComponent] = parameters;
            }
        }

        // Perform the lerping
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            foreach (var componentEntry in effectParameters)
            {
                var component = componentEntry.Key;
                foreach (var paramEntry in componentEntry.Value)
                {
                    var field = component.GetType().GetField(paramEntry.Key, BindingFlags.Public | BindingFlags.Instance);

                    if (field.FieldType == typeof(ClampedFloatParameter) || field.FieldType == typeof(FloatParameter))
                    {
                        var parameter = (FloatParameter)field.GetValue(component);
                        parameter.value = Mathf.Lerp((float)paramEntry.Value.startValue, (float)paramEntry.Value.targetValue, t);
                    }
                    else if (field.FieldType == typeof(ColorParameter))
                    {
                        var parameter = (ColorParameter)field.GetValue(component);
                        parameter.value = Color.Lerp((Color)paramEntry.Value.startValue, (Color)paramEntry.Value.targetValue, t);
                    }
                }
            }

            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Set all parameters to their final target values
        foreach (var componentEntry in effectParameters)
        {
            var component = componentEntry.Key;
            foreach (var paramEntry in componentEntry.Value)
            {
                var field = component.GetType().GetField(paramEntry.Key, BindingFlags.Public | BindingFlags.Instance);

                if (field.FieldType == typeof(ClampedFloatParameter) || field.FieldType == typeof(FloatParameter))
                {
                    var parameter = (FloatParameter)field.GetValue(component);
                    parameter.value = (float)paramEntry.Value.targetValue;
                }
                else if (field.FieldType == typeof(ColorParameter))
                {
                    var parameter = (ColorParameter)field.GetValue(component);
                    parameter.value = (Color)paramEntry.Value.targetValue;
                }
            }
        }

        // Optional: Set the entire profile to the new profile at the end of the transition
        currentVolume.profile = newProfile;
    }

    // Optional reset method to revert to the original profile
    public void ResetToOriginalProfile(float transitionDuration)
    {
        StartCoroutine(LerpAllEffectsToNewProfile(originalProfile, transitionDuration));
    }
}