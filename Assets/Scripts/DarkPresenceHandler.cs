using UnityEngine;
using System.Collections;

public class DarkPresenceHandler : MonoBehaviour
{
    VignetteController vignetteController;

    private void Start()
    {
        vignetteController = GetComponentInChildren<VignetteController>();
    }
    public void DarkPresence(bool kill)
    {

        if (kill)
            vignetteController.StartEffect(1f);
        else
            vignetteController.StartEffect(0.3f);
        //Increase Vignette on player if true, if false, decrease vignette until a certain threshold, like 0.3f.
        //Start Heartbeat low and increase over time if true, if false, stop with fading out
    }

    public void ReturnToNormal()
    {

        //Return vignette to normal and kill heartbeat
        vignetteController.StartEffect(0f);
    }    


}
