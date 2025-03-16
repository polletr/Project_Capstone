using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemyTrigger : MonoBehaviour
{
    public GameEvent Event;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");
        if (other.TryGetComponent(out NurseEnemy nurse))
        {
            nurse.currentState?.HandleRetreat();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            Event.PlayerExitedSafeZone?.Invoke(player);
        }
    }

    public void BroadcastPlayerProgress(int progress)
    {
        Event.PlayerEnteredSafeZone?.Invoke(progress);
    }

}
