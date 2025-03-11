using System.Collections;
using UnityEngine;

public class DarkTrigger : MonoBehaviour
{
    PlayerController playerController;

    private void OnTriggerStay(Collider other)
    {
        if (!other.GetComponent<PlayerController>())
            return;
        else
            playerController = other.GetComponent<PlayerController>();


        if (!playerController.GetComponentInChildren<FlashLight>().IsFlashlightOn)
        {
            playerController.DarkPresence(true);
        }
        else
        {
            playerController.DarkPresence(false);
        }
    }

}
