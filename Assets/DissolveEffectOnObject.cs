using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DissolveEffectOnObject : MonoBehaviour
{
    [SerializeField]
    private float timer = 1f;

    [SerializeField]
    private Material dissolveMaterial;

    private Dictionary<MeshRenderer, Material[]> originalMaterials = new Dictionary<MeshRenderer, Material[]>();
    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    private void Awake()
    {
        // Cache all MeshRenderers and their materials
        meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        foreach (var renderer in meshRenderers)
        {
            originalMaterials[renderer] = renderer.materials;
        }
    }

    public void EnableObj()
    {
        gameObject.SetActive(true);

        // Apply dissolve material to all renderers
        foreach (var renderer in meshRenderers)
        {
            Material[] dissolveMats = new Material[renderer.materials.Length];
            for (int i = 0; i < dissolveMats.Length; i++)
            {
                dissolveMats[i] = new Material(dissolveMaterial); // Create a new instance of the dissolve material
            }
            renderer.materials = dissolveMats;
        }

        // Start the appear effect
        StartCoroutine(AppearCoroutine());
    }

    public void DisableObj()
    {
        // Apply dissolve material to all renderers
        foreach (var renderer in meshRenderers)
        {
            Material[] dissolveMats = new Material[renderer.materials.Length];
            for (int i = 0; i < dissolveMats.Length; i++)
            {
                dissolveMats[i] = new Material(dissolveMaterial); // Create a new instance of the dissolve material
            }
            renderer.materials = dissolveMats;
        }

        // Start the dissolve effect
        StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator AppearCoroutine()
    {
        float currentDissolveAmount = 1f;

        while (currentDissolveAmount > 0f)
        {
            currentDissolveAmount -= Time.deltaTime / timer;
            foreach (var renderer in meshRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_DissolveAmount", Mathf.Clamp01(currentDissolveAmount));
                }
            }

            yield return null;
        }

        // Restore the original materials
        foreach (var renderer in meshRenderers)
        {
            renderer.materials = originalMaterials[renderer];
        }
    }

    private IEnumerator DissolveCoroutine()
    {
        float currentDissolveAmount = 0f;

        while (currentDissolveAmount < 1f)
        {
            currentDissolveAmount += Time.deltaTime / timer;
            foreach (var renderer in meshRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_DissolveAmount", Mathf.Clamp01(currentDissolveAmount));
                }
            }

            yield return null;
        }

        // Fully dissolve and disable the object
        gameObject.SetActive(false);
    }
}
