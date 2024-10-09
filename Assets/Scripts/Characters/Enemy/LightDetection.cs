using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightDetection : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 8f;
    private LayerMask lightLayerMask;

    private HashSet<LightController> turnedOffLights = new HashSet<LightController>();

    private void Start()
    {
        lightLayerMask = LayerMask.GetMask("LightController");
    }

    private void Update()
    {
        // Check for overlapping colliders within the detection radius
        Collider[] lightColliders = Physics.OverlapSphere(transform.position, detectionRadius, lightLayerMask);

        // Create a set for currently detected light controllers
        HashSet<LightController> currentLights = new HashSet<LightController>();

        foreach (var collider in lightColliders)
        {
            if (collider.TryGetComponent(out LightController lightController))
            {
                currentLights.Add(lightController);
                // Turn off the light if it hasn't been turned off yet
                if (!turnedOffLights.Contains(lightController))
                {
                    if (lightController.transform.position.y >= transform.position.y)
                    {
                        lightController.TurnOnOffLight(false);
                        turnedOffLights.Add(lightController); // Keep track of turned off lights
                    }
                }
            }
        }

        // Check for lights that should be turned back on
        foreach (var lightController in turnedOffLights)
        {
            if (!currentLights.Contains(lightController))
            {
                lightController.WaitAndTurnOnLight(10f);
            }
        }

        // Clear the turned off lights list if they are now back on
        turnedOffLights.RemoveWhere(light => !currentLights.Contains(light));
    }
}
