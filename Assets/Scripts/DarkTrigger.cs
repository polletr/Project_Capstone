using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;

public class DarkTrigger : MonoBehaviour
{
    DarkPresenceHandler darkPresenceHandler;

    FlashLight flashLight;
    [SerializeField] private GameObject[] objectsToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<DarkPresenceHandler>(out darkPresenceHandler))
            return;

        flashLight = other.GetComponentInChildren<FlashLight>();

        foreach (var obj in objectsToDisable)
        {
            obj.SetActive(false);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (darkPresenceHandler == null)
            return;

        darkPresenceHandler.DarkPresence(!flashLight.IsFlashlightOn);
    }

    private void OnTriggerExit(Collider other)
    {
        if (darkPresenceHandler == null)
            return;

        darkPresenceHandler.ReturnToNormal();
        darkPresenceHandler = null;

        foreach (var obj in objectsToDisable)
        {
            obj.SetActive(true);
        }

    }
}
