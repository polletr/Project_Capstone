using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearDisappear : MonoBehaviour
{
    [SerializeField] GameObject appearObj;
    [SerializeField] GameObject disappearObj;
    public void Appear()
    {
        disappearObj?.SetActive(false);
        appearObj?.SetActive(true);
    }

    public void Disappear()
    {
        appearObj?.SetActive(false);
        disappearObj?.SetActive(true);
    }

}
