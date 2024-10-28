using FMOD.Studio;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class DisapearObject : MonoBehaviour, IHideable 
{
    private List<MeshRenderer> meshRenderers = new ();
    private Dictionary<MeshRenderer, Material[]> originalMaterials = new ();

    [SerializeField] private Material ObjMaterial;
    [SerializeField] private Material dissolveMaterial; 
    [SerializeField] private float hideTime;
    [SerializeField] private float unHideTime;
    [SerializeField] private UnityEvent objectRevealed;
    [SerializeField] private UnityEvent OnApplyEffect;
    [SerializeField] private UnityEvent OnRemoveEffect;


    private EventInstance revealSound;

    public bool IsHidden { get; set; }

    private float hideTimer;
    private float currentObjTransp;

    private void Awake()
    {
        // Find and store all MeshRenderers in the object's hierarchy
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());

        // Store original materials for each MeshRenderer
        foreach (var renderer in meshRenderers)
        {
            originalMaterials[renderer] = renderer.materials;
            renderer.material = ObjMaterial;
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
        HideFunction();
        revealed = IsHidden;
    }

    private void HideFunction()
    {

        if (!IsHidden)
        {
            // Set dissolve material for all renderers
            SetMaterials(dissolveMaterial);

            // Play reveal sound if not already playing
            revealSound.getPlaybackState(out var playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                revealSound.start();
            }

            hideTimer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(0f,1f, hideTimer / hideTime);

            foreach (var material in meshRenderers.SelectMany(renderer => renderer.materials))
            {
                material.SetFloat("_DissolveAmount", currentObjTransp); 
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
            IsHidden = true;
        }
    }

    public void HideObj()
    {
        StopAllCoroutines();
        StartCoroutine(UnHideCoroutine());
    }

    private IEnumerator UnHideCoroutine()
    {
        hideTimer = 0f;
        revealSound.stop(STOP_MODE.IMMEDIATE);

        var startTransparency = currentObjTransp;

        while (currentObjTransp > 0f)
        {
            hideTimer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(startTransparency, 0f, hideTimer / unHideTime);

            foreach (var material in meshRenderers.SelectMany(renderer => renderer.materials))
            {
                material.SetFloat("_DissolveAmount", currentObjTransp);
            }

            yield return null;
        }

        SetMaterials(ObjMaterial);
        IsHidden = false;
    }

    private void SetMaterials(Material material)
    {
        foreach (var renderer in meshRenderers)
        {
            var materials = new Material[renderer.materials.Length];
            for (var i = 0; i < materials.Length; i++)
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
            if (!originalMaterials.TryGetValue(renderer, out var material)) continue;
            
            renderer.materials = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }
}