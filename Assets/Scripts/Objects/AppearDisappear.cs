using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearDisappear : MonoBehaviour
{
    [SerializeField] List<GameObject> appearObj = new();
    [SerializeField] List<GameObject> disappearObj = new();
    public void Appear()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.SuddenAppear, this.transform.position);

        foreach (GameObject obj in appearObj)
        {
            obj?.SetActive(true);
        }
        foreach (GameObject obj in disappearObj)
        {
            obj?.SetActive(false);
        }

    }


    public void RevertAppear()
    {
        foreach (GameObject obj in appearObj)
        {
            obj?.SetActive(false);
        }
        foreach (GameObject obj in disappearObj)
        {
            obj?.SetActive(true);
        }
    }

}
