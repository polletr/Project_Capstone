using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class ShadowObject : MonoBehaviour, IStunable
{
    public bool CanApplyEffect { get; set; }

    [SerializeField] UnityEvent StunObject;

    [SerializeField] Material shadowMaterial;  // The shadow material to apply
    [SerializeField] float changeTranspDuration = 1f;  // Duration for transparency change

    Outline outline;
    [SerializeField] bool applyOutline = true;
    [SerializeField] float cooldownTime;

    Coroutine enemyTransp;

    CountdownTimer countdownTimer;

    private List<Material> originalMaterials = new List<Material>();  // List to store original materials

    private void Start()
    {
        CanApplyEffect = true;
        outline = GetComponent<Outline>();

        // Grab all materials from the object and change them to shadow material
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                originalMaterials.Add(mat);  // Save the original material
            }

            Material[] shadowMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < shadowMaterials.Length; i++)
            {
                shadowMaterials[i] = shadowMaterial;  // Replace with shadow material
            }

            renderer.materials = shadowMaterials;
        }
    }

    public void ApplyStunEffect()
    {
        Debug.Log("Enemy Received Stun");
        if (enemyTransp == null)
        {
            StartCoroutine(EnemyTransparency(0f));
            StunObject.Invoke();
        }
    }

    public void ApplyEffect()
    {
        if (applyOutline)
            outline.AppyOutlineEffect();
    }

    public void RemoveEffect()
    {
        if (applyOutline)
            outline.RemoveOutlineEffect();
    }

    private IEnumerator StunCooldown()
    {
        countdownTimer = new CountdownTimer(cooldownTime);
        countdownTimer.Start();
        while (!countdownTimer.IsFinished)
        {
            countdownTimer.Tick(Time.deltaTime);
            yield return null;
        }
        StartCoroutine(EnemyTransparency(0.9f));

        yield return null;
    }

    private IEnumerator EnemyTransparency(float targetTransp)
    {
        float elapsedTime = 0f;

        // Store starting transparency values for each original material
        List<float> startTransparencyValues = new List<float>();
        foreach (Material mat in originalMaterials)
        {
            startTransparencyValues.Add(mat.GetFloat("_Transparency"));
        }

        // Gradually change transparency over the duration
        while (elapsedTime < changeTranspDuration)
        {
            for (int i = 0; i < originalMaterials.Count; i++)
            {
                float startTransparency = startTransparencyValues[i];
                float newTransparency = Mathf.Lerp(startTransparency, targetTransp, elapsedTime / changeTranspDuration);
                originalMaterials[i].SetFloat("_Transparency", newTransparency);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the transparency is set to the target value at the end
        foreach (Material mat in originalMaterials)
        {
            mat.SetFloat("_Transparency", targetTransp);
        }
        if (targetTransp <= 0f)
        {
            GetComponent<Collider>().enabled = false;
        }
        else
        {
            GetComponent<Collider>().enabled = true;
        }

        StartCoroutine(StunCooldown());
    }
}
