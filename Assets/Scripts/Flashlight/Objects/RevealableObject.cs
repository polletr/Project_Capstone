using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealableObject : MonoBehaviour  , IRevealable
{
    private Material objMaterial;
    [SerializeField] private float revealTime;
    [SerializeField] private float unRevealTime;

    float revealTimer;
    private float currentObjTransp;
    private float origObjTransp;

    public bool isRevealed {  get; private set; }

    void Awake()
    {
        objMaterial = GetComponent<MeshRenderer>().material;
        origObjTransp = objMaterial.GetFloat("_Transparency");
        revealTimer = 0f;

        currentObjTransp = origObjTransp;
    }

    public void ApplyEffect()
    {
        GetComponent<MeshRenderer>().material = objMaterial;
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
        currentObjTransp = Mathf.Lerp(origObjTransp, 1f, revealTimer / revealTime);
        Debug.Log(revealTimer);
        objMaterial.SetFloat("_Transparency", currentObjTransp);
        if (revealTimer >= revealTime)
        {
            revealTimer = 0f;
            isRevealed = true;
        }
        revealed = currentObjTransp >= 1f;
    }

    public void UnrevealObj()
    {
        StartCoroutine(Unreveal());
    }


    private IEnumerator Unreveal()
    {
        revealTimer = 0f;

        float currentFloat = currentObjTransp;
        float timer = 0f;
        while (currentObjTransp > origObjTransp)
        {
            timer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(currentFloat, origObjTransp, timer / unRevealTime);
            objMaterial.SetFloat("_Transparency", currentObjTransp);

            yield return null;
        }
    }

}
