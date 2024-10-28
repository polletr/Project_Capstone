using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoorClose : MonoBehaviour
{
   // [SerializeField] private Door doorScript;

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.GetComponent<PlayerController>())
    //     {
    //         Debug.Log("Call Close");
    //         doorScript?.OnCloseDoor(5f);
    //     }
    // }
    public void DoorCloserEntered(Collider other, Vector3 forward)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            var cController = other.GetComponent<CharacterController>();
            var newVelocity = new Vector3(cController.velocity.z, 0, cController.velocity.x);
            if (Vector3.Dot( newVelocity, forward) < 0)
                return;
            Door door = GetComponent<Door>();
            if (door == null || !door.isActiveAndEnabled)
                return;
            if(!door.Rotating && door.IsOpen)
                door?.OnCloseDoor(5f);
        }
    }
}
