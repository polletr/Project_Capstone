using UnityEngine;
using System.Collections;

public class DarkPresenceHandler : MonoBehaviour
{
    Coroutine returnToNormal;
    public void DarkPresence(bool kill)
    {
        if (returnToNormal != null)
            StopCoroutine(returnToNormal);

        //Increase Vignette on player if true, if false, decrease vignette until a certain threshold, like 0.3f.
        //Start Heartbeat low and increase over time if true, if false, stop with fading out
    }

    public void ReturnToNormal()
    {
        if (returnToNormal != null)
            StopCoroutine(returnToNormal);

        returnToNormal = StartCoroutine(StartReturnToNormal());
    }    

    private IEnumerator StartReturnToNormal()
    {

        //Return vignette to normal and kill heartbeat
        yield return null;
    }

}
