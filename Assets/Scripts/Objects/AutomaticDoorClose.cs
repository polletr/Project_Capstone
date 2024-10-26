using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoorClose : MonoBehaviour
{
    [SerializeField] private Door doorScript;

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            Debug.Log("Call Close");
            doorScript?.OnCloseDoor(5f);
        }
    }
}
