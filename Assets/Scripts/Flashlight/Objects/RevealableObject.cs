using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RevealableObject : MonoBehaviour  , IRevealable
{
    private MeshRenderer m_Renderer;
    private Material objMaterial;

    private Material originalMaterial;
    [SerializeField] private Material revealMaterial;
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float revealTime;
    [SerializeField] private float unRevealTime;

    [SerializeField] private UnityEvent objectRevealed;

    public bool IsRevealed
    {
        get;
        set;
    }

    private float revealTimer;
    private float currentObjTransp;
    private float origObjTransp;

    void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        
        originalMaterial = m_Renderer.material;
        m_Renderer.material = revealMaterial;
        objMaterial = revealMaterial;
            revealTimer = 0f;
    }

    public void ApplyEffect()
    {
        
    }

    public void RemoveEffect()
    {

    }

    public void SuddenReveal()
    {
        gameObject.SetActive(true);
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.SuddenAppear, this.transform.position);
    }

    public void RevealObj(out bool revealed)
    {
        StopAllCoroutines();
        revealTimer += Time.deltaTime;
        m_Renderer.material = dissolveMaterial;
        currentObjTransp = Mathf.Lerp(1f, 0f, revealTimer / revealTime);
        m_Renderer.material.SetFloat("_DissolveAmount", currentObjTransp);
        if (revealTimer >= revealTime)
        {
            revealTimer = 0f;
        }

        if (currentObjTransp <= 0f)
        {
            IsRevealed = true;
            m_Renderer.material = originalMaterial;
            objectRevealed.Invoke();
        }
        revealed = currentObjTransp <= 0f;
   }

    public void UnRevealObj()
    {
        StartCoroutine(UnReveal());
    }


    private IEnumerator UnReveal()
    {
        revealTimer = 0f;

        var currentFloat = currentObjTransp;
        var timer = 0f;
        while (currentObjTransp < 1f)
        {
            timer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(currentFloat, 1f, timer / unRevealTime);
            m_Renderer.material.SetFloat("_DissolveAmount", currentObjTransp);

            yield return null;
        }
        m_Renderer.material = revealMaterial;
    }

}
