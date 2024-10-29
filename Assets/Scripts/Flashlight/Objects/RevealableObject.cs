using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RevealableObject : MonoBehaviour, IRevealable
{
    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private Dictionary<MeshRenderer, Material[]> originalMaterials = new Dictionary<MeshRenderer, Material[]>();

    [SerializeField] private Material revealMaterial;
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float revealTime;
    [SerializeField] private float unRevealTime;
    [SerializeField] private UnityEvent objectRevealed;
    [SerializeField] private UnityEvent OnApplyEffect;
    [SerializeField] private UnityEvent OnRemoveEffect;


    private EventInstance revealSound;

    public bool IsRevealed { get; set; }

    private float revealTimer;
    private float currentObjTransp;

    void Awake()
    {
        // Find and store all MeshRenderers in the object's hierarchy
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());

        // Store original materials for each MeshRenderer
        foreach (var renderer in meshRenderers)
        {
            originalMaterials[renderer] = renderer.materials;
            renderer.material = revealMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        revealSound = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.FlashlightRevealing);
    }

    public void ApplyEffect()
    {
        OnApplyEffect.Invoke();
    }

    public void RemoveEffect()
    {
        OnRemoveEffect.Invoke();
    }

    public void SuddenReveal()
    {
        gameObject.SetActive(true);
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.SuddenAppear, this.transform.position);
    }

    public void RevealObj(out bool revealed)
    {
        StopAllCoroutines();
        RevealFunction();
        revealed = IsRevealed;
    }

    private void RevealFunction()
    {

        if (!IsRevealed)
        {
            // Set dissolve material for all renderers
            SetMaterials(dissolveMaterial);

            // Play reveal sound if not already playing
            PLAYBACK_STATE playbackState;
            revealSound.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                revealSound.start();
            }

            revealTimer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(1f, 0f, revealTimer / revealTime);

            foreach (var renderer in meshRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_DissolveAmount", currentObjTransp);
                }
            }
        }

        if (currentObjTransp <= 0f)
        {
            // After revealing, set original materials and stop sound
            SetOriginalMaterials();
            if (GetComponent<IndicatorHandler>() != null)
            {
                GetComponent<IndicatorHandler>().enabled = true;
            }
            revealSound.stop(STOP_MODE.IMMEDIATE);
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.FlashlightReveal, this.transform.position);
            objectRevealed.Invoke();
            IsRevealed = true;
        }
    }

    public void UnRevealObj()
    {
        StopAllCoroutines();
        StartCoroutine(UnRevealCoroutine());
    }

    private IEnumerator UnRevealCoroutine()
    {
        revealTimer = 0f;
        revealSound.stop(STOP_MODE.IMMEDIATE);

        float startTransparency = currentObjTransp;

        while (currentObjTransp < 1f)
        {
            revealTimer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(startTransparency, 1f, revealTimer / unRevealTime);

            foreach (var renderer in meshRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_DissolveAmount", currentObjTransp);
                }
            }

            yield return null;
        }

        SetMaterials(revealMaterial);
        IsRevealed = false;
    }

    private void SetMaterials(Material material)
    {
        foreach (var renderer in meshRenderers)
        {
            var materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }
            renderer.materials = materials;
        }
    }

    private void SetOriginalMaterials()
    {
        foreach (var renderer in meshRenderers)
        {
            if (originalMaterials.ContainsKey(renderer))
            {
                renderer.materials = originalMaterials[renderer];
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }
}