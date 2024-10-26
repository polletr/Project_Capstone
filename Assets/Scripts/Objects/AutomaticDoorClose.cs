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
            doorScript?.OnCloseDoor(5f);
        }
    }
}
