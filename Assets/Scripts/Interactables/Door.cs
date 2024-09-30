using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    public GameEvent Event;
    [SerializeField] private bool isLocked;
    [SerializeField] private UnityEvent OnOpen;
    [SerializeField] private UnityEvent OnClose;
    [SerializeField] private UnityEvent OnUnlock;
    [SerializeField] private UnityEvent OnInteractLocked;

    [SerializeField] private float rotationAngle = 90f;  // Angle to open the door
    [SerializeField] private float rotationSpeed = 2f;   // Speed of rotation

    private Quaternion closedRotation;
    private bool isOpen = false;  // To track door state

    private Camera playerCamera;

    private NavMeshObstacle doorObstacle; // Assign in Inspector

    private ShakeEffect shakeEffect;

    private void Start()
    {
        shakeEffect = GetComponent<ShakeEffect>();
        // Save the door's closed rotation
        doorObstacle = GetComponent<NavMeshObstacle>();
        closedRotation = transform.localRotation;
        playerCamera = Camera.main;
    }

    public void OnInteract()
    {
        if (!isLocked)
        {
            if (isOpen)
            {
                // Close the door
                StartCoroutine(RotateDoor(closedRotation, rotationSpeed));
                AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.CloseDoor, this.transform.position);
                OnClose.Invoke();
            }
            else
            {
                // Open the door
                StartCoroutine(RotateDoor(CheckDirection(), rotationSpeed));
                AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.OpenDoor, this.transform.position);
                OnOpen.Invoke();
            }

            // Toggle the door state
            isOpen = !isOpen;
        }
        else
        {
            //if player has key
            //UnlockDoor();
            //if player doesnt have key:
            shakeEffect.ShakeObject();
            OnInteractLocked.Invoke();
            //Event.OnTryToUnlockDoor?.Invoke(this);
        }
    }

    private void Update()
    {
        Debug.Log("Door is locked" + isLocked);
    }

    public void OnTriggerOpen(float speed)
    {
        if (!isOpen)
        {
            StartCoroutine(RotateDoor(CheckDirection(), speed));
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.OpenDoor, this.transform.position);
            OnOpen.Invoke();
        }
    }

    public void OnTriggerClose(float speed)
    {
        if (isOpen)
        {
            // Close the door
            StartCoroutine(RotateDoor(closedRotation, speed));
            OnClose.Invoke();
        }
    }

    private Quaternion CheckDirection()
    {
        // Open the door forward relative to the player
        Vector3 doorToPlayer = playerCamera.transform.position - transform.position;
        Vector3 doorForward = transform.right;

        // Check if the player is behind the door to determine opening direction
        float dotProduct = Vector3.Dot(doorForward, doorToPlayer);

        // Determine the target rotation to open the door forward
        if (dotProduct > 0)
        {
            // Player is behind the door, so open forward
            return Quaternion.Euler(0, -rotationAngle, 0) * closedRotation;
        }
        else
        {
            // Player is in front of the door, open backwards
            return Quaternion.Euler(0, rotationAngle, 0) * closedRotation;
        }

    }

    public void UnlockDoor()
    {
        OnUnlock.Invoke();
        isLocked = false;
    }

    private IEnumerator RotateDoor(Quaternion targetRotation, float speed)
    {
        float timeElapsed = 0f;
        Quaternion currentRotation = transform.localRotation;

        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime * speed;
            transform.localRotation = Quaternion.Slerp(currentRotation, targetRotation, timeElapsed);
            yield return null;
        }

        // Ensure the rotation is exactly the target rotation at the end
        transform.localRotation = targetRotation;

        if (isOpen)
        {
            doorObstacle.carving = false;
        }
        else
        {
            doorObstacle.carving = true;
        }
    }
}
