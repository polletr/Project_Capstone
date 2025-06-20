using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Outline))]
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
    [field:SerializeField] public bool ApplyOutline {get; set; }= true;

    private EventInstance revealSound;

    bool originalTrigger;

    public bool IsRevealed { get; set; }
    public bool CanApplyEffect { get; set; }


    private float revealTimer;
    private float currentObjTransp;

    private Outline outline;

    private void Awake()
    {
        originalTrigger = GetComponent<Collider>().isTrigger;

        // Find and store all MeshRenderers in the object's hierarchy
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());

        // Store original materials for each MeshRenderer
        foreach (var renderer in meshRenderers)
        {
            originalMaterials[renderer] = renderer.materials;
        }

        outline = GetComponent<Outline>();
        revealSound = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.FlashlightRevealing);

    }

    void OnEnable()
    {
        foreach (var renderer in meshRenderers)
        {
            renderer.material = revealMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        CanApplyEffect = true;


        IsRevealed = false;
        GetComponent<Collider>().isTrigger = true;

        if (TryGetComponent(out DisappearObject disapearObject))
            disapearObject.enabled = false;

    }

    void OnDisable()
    {
        CanApplyEffect = true;
    }

    public void ApplyEffect()
    {
        if (!IsRevealed)
        {
            OnApplyEffect.Invoke();
            if (ApplyOutline)
                outline.AppyOutlineEffect();

        }
    }

    public void RemoveEffect()
    {
        if (!IsRevealed)
        {
            OnRemoveEffect.Invoke();
            if (ApplyOutline)
                outline.RemoveOutlineEffect();
        }
    }

    public void SuddenReveal()
    {
        gameObject.SetActive(true);
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.SuddenAppear,
            this.transform.position);
    }

    public void RevealObj(out bool revealed)
    {
        StopAllCoroutines();
        StartCoroutine(RevealFunction());
        revealed = IsRevealed;
    }

    private IEnumerator RevealFunction()
    {
        outline.RemoveOutlineEffect();
        currentObjTransp = 1f;
        revealTimer = 0f;

        if (!IsRevealed)
        {

            CanApplyEffect = false;
            // Set dissolve material for all renderers
            SetMaterials(dissolveMaterial);

            // Play reveal sound if not already playing
            revealSound.getPlaybackState(out var playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                revealSound.start();
            }

            while (currentObjTransp > 0f)
            {
                revealTimer += Time.deltaTime;
                currentObjTransp = Mathf.Lerp(1f, 0f, revealTimer / revealTime);

                foreach (var material in meshRenderers.SelectMany(renderer => renderer.materials))
                {
                    material.SetFloat("_DissolveAmount", currentObjTransp);
                }
                yield return null;
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
                AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.FlashlightReveal,
                    this.transform.position);
                IsRevealed = true;
                 objectRevealed.Invoke();

                if (TryGetComponent(out NavMeshObstacle obstacle))
                {
                    obstacle.enabled = true;
                }

                if (TryGetComponent(out DisappearObject disapearObject))
                {
                    disapearObject.enabled = true;
                }

                GetComponent<Collider>().isTrigger = originalTrigger;
            }
        }

    }

    public void UnRevealObj()
    {
        StopAllCoroutines();
        if(this.gameObject.activeSelf)
            StartCoroutine(UnRevealCoroutine());
    }

    private IEnumerator UnRevealCoroutine()
    {
        revealTimer = 0f;
        revealSound.stop(STOP_MODE.IMMEDIATE);

        var startTransparency = currentObjTransp;

        while (currentObjTransp < 1f)
        {
            revealTimer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(startTransparency, 1f, revealTimer / unRevealTime);

            foreach (var material in meshRenderers.SelectMany(renderer => renderer.materials))
            {
                material.SetFloat("_DissolveAmount", currentObjTransp);
            }

            yield return null;
        }
        revealTimer = 0f;

        SetMaterials(revealMaterial);
        CanApplyEffect = true;
        IsRevealed = false;
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
        RemoveEffect();

    }

    private void SetOriginalMaterials()
    {
        foreach (var renderer in meshRenderers)
        {
            if (originalMaterials.TryGetValue(renderer, out var material))
            {
                renderer.materials = material;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
        RemoveEffect();

    }
}