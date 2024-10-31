
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DoorCloseTrigger : MonoBehaviour
{
    [SerializeField]
    DoorCloseTrigger otherTrigger;
    [SerializeField] Door door;

    public UnityEvent UTriggerEnter, UTriggerStay, UTriggerExit;
    public bool triggerEnabled { get; set; }
    private void Start()
    {
        triggerEnabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            UTriggerEnter.Invoke();
            if (door.IsOpen && triggerEnabled)
            {
                Debug.Log("Close");
                door.GetComponent<AutomaticDoorClose>().StartCountdownToCloseDoor();
                otherTrigger.triggerEnabled = true;
            }

            if (!door.IsOpen && triggerEnabled)
            {
                triggerEnabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            UTriggerExit.Invoke();

            if (!door.IsOpen)
            {
                triggerEnabled = true;
            }
        }
    }

}
