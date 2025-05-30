using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]

public class DisappearObject : MonoBehaviour, IHideable
{
       private List<MeshRenderer> meshRenderers = new ();
       private Dictionary<MeshRenderer, Material[]> originalMaterials = new Dictionary<MeshRenderer, Material[]>();

    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float revealTime;
    [SerializeField] private float unRevealTime;
    [SerializeField] private UnityEvent objectRevealed;
    [SerializeField] private UnityEvent OnApplyEffect;
    [SerializeField] private UnityEvent OnRemoveEffect;


    private EventInstance revealSound;

    public bool CanApplyEffect { get; set; }
    public bool IsHidden { get; set; }

    private float revealTimer;
    private float currentObjTransp = 0f;

    private Outline outline;

    [SerializeField] bool applyOutline = true;

    private void Awake()
    {
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

    private void OnEnable()
    {
        IsHidden = false;
        GetComponent<Collider>().isTrigger = false;
        revealTimer = 0f;

        CanApplyEffect = true;

        if (TryGetComponent(out RevealableObject revealableObject))
            revealableObject.enabled = false;
    }

    private void OnDisable()
    {
        CanApplyEffect = false;
    }

    public void ApplyEffect()
    {
        OnApplyEffect.Invoke();
        if (applyOutline)
            outline.AppyOutlineEffect();
    }

    public void RemoveEffect()
    {
        OnRemoveEffect.Invoke();
        if (applyOutline)
            outline.RemoveOutlineEffect();
    }

    public void HideObj(out bool revealed)
    {
        StopAllCoroutines();
        StartCoroutine(HideFunction());
        revealed = IsHidden;
    }

    private IEnumerator HideFunction()
    {
        outline.RemoveOutlineEffect();
        currentObjTransp = 0f;
        revealTimer = 0f;
        if (!IsHidden)
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

            while (currentObjTransp <0.75f)
            {
                revealTimer += Time.deltaTime;
                currentObjTransp = Mathf.Lerp(0, 0.75f, revealTimer / revealTime);

                foreach (var material in meshRenderers.SelectMany(renderer => renderer.materials))
                {
                    material.SetFloat("_DissolveAmount", currentObjTransp);
                }
                yield return null;
            }

            if (currentObjTransp >= 0.75f)
            {
                revealSound.stop(STOP_MODE.IMMEDIATE);
                AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.FlashlightReveal,
                    this.transform.position);
                objectRevealed.Invoke();
                IsHidden = true;
                if (TryGetComponent(out NavMeshObstacle obstacle))
                {
                    obstacle.enabled = false;
                }

                if (TryGetComponent(out RevealableObject revealableObject))
                {
                    revealableObject.enabled = true;
                }
                else
                {
                    Destroy(gameObject);
                }
            }

        }

    }

    public void UnHideObj()
    {
        StopAllCoroutines();
        if (this.gameObject.activeSelf)
            StartCoroutine(UnhideCoroutine());
    }

    private IEnumerator UnhideCoroutine()
    {
        revealTimer = 0f;
        revealSound.stop(STOP_MODE.IMMEDIATE);

        var startTransparency = currentObjTransp;

        while (currentObjTransp > 0f)
        {
            revealTimer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(startTransparency, 0f, revealTimer / unRevealTime);

            foreach (var material in meshRenderers.SelectMany(renderer => renderer.materials))
            {
                material.SetFloat("_DissolveAmount", currentObjTransp);
            }

            yield return null;
        }
        revealTimer = 0f;

        SetOriginalMaterials();
        CanApplyEffect = true;
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