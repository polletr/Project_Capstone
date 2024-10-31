using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class AutomaticDoorClose : MonoBehaviour
{
    [SerializeField] DoorCloseTrigger triggerA;
    [SerializeField] DoorCloseTrigger triggerB;
    [SerializeField] float timeToCloseDoor;

    Door door;
    private CountdownTimer countdownTimer;
    private Coroutine closeDoorCoroutine;
    private void Awake()
    {
        door = GetComponent<Door>();
        countdownTimer = new CountdownTimer(timeToCloseDoor);
    }

    public void StartCountdownToCloseDoor()
    {
        if (door.IsOpen)
        {
            if (closeDoorCoroutine != null)
            {
                StopCoroutine(closeDoorCoroutine);
            }
            closeDoorCoroutine = StartCoroutine(CloseDoorAfterCountdown());
        }
    }

    public void StopCountdownToCloseDoor()
    {
        if (closeDoorCoroutine != null)
        {
            StopCoroutine(closeDoorCoroutine);
            closeDoorCoroutine = null;
            countdownTimer.Reset();
        }
    }
    private IEnumerator CloseDoorAfterCountdown()
    {
        countdownTimer.Start();

        // Wait until the timer is finished
        while (!countdownTimer.IsFinished)
        {
            countdownTimer.Tick(Time.deltaTime);
            yield return null;
        }

        // Timer is finished, close the door
        if (door != null && door.IsOpen)
        {
            door.OnCloseDoor();
            Debug.Log("Door is closing.");
        }

        closeDoorCoroutine = null;
    }

}
