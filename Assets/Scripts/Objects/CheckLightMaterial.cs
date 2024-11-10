using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLightMaterial : MonoBehaviour
{

    private List<Material> emissiveMaterials = new List<Material>();
    private Light lightController;

    void Start()
    {
        // Find all renderers in the children and check for emissive materials
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                // Check if material has emission enabled
                if (material.IsKeywordEnabled("_EMISSION"))
                {
                    emissiveMaterials.Add(material);
                }
            }
        }

        // Check for a Light component within this object
        lightController = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        ChangeLight(lightController.enabled);
    }

    public void ChangeLight(bool lightPower)
    {
        if (lightPower)
        {
            // Turn on emission for each emissive material
            foreach (Material material in emissiveMaterials)
            {
                material.EnableKeyword("_EMISSION");
            }
        }
        else
        {
            // Turn off emission for each emissive material
            foreach (Material material in emissiveMaterials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}
