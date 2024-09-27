using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isLocked;
    [SerializeField] private UnityEvent OnOpen;
    [SerializeField] private UnityEvent OnClose;
    [SerializeField] private UnityEvent OnUnlock;
    [SerializeField] private UnityEvent OnInteractLocked;

    [SerializeField] private float rotationAngle = 90f;  // Angle to open the door
    [SerializeField] private float rotationSpeed = 2f;   // Speed of rotation

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;  // To track door state

    private Camera playerCamera;

    private void Start()
    {
        // Save the door's closed rotation
        closedRotation = transform.localRotation;
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    public void OnInteract()
    {
        if (!isLocked)
        {
            if (isOpen)
            {
                // Close the door
                StartCoroutine(RotateDoor(closedRotation));
                OnClose.Invoke();
            }
            else
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
                    openRotation = Quaternion.Euler(0, -rotationAngle, 0) * closedRotation;
                }
                else
                {
                    // Player is in front of the door, open backwards
                    openRotation = Quaternion.Euler(0, rotationAngle, 0) * closedRotation;
                }

                // Open the door
                StartCoroutine(RotateDoor(openRotation));
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
            OnInteractLocked.Invoke();
        }
    }

    public void UnlockDoor()
    {
        OnUnlock.Invoke();
        isLocked = false;
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        float timeElapsed = 0f;
        Quaternion currentRotation = transform.localRotation;

        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime * rotationSpeed;
            transform.localRotation = Quaternion.Slerp(currentRotation, targetRotation, timeElapsed);
            yield return null;
        }

        // Ensure the rotation is exactly the target rotation at the end
        transform.localRotation = targetRotation;
    }
}
