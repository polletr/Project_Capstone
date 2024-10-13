using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteractableIndicator : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position + offset);
        if(transform.position != screenPos)
        transform.position = screenPos;
    }
}
