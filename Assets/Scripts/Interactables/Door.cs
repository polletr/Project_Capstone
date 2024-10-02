using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    public GameEvent Event;
    public bool IsOpen {  get; private set; }
    [SerializeField] private bool isLocked;
    [SerializeField] private UnityEvent OnOpen;
    [SerializeField] private UnityEvent OnClose;
    [SerializeField] private UnityEvent OnUnlock;
    [SerializeField] private UnityEvent OnInteractLocked;

    [SerializeField] private float rotationAngle = 90f;  // Angle to open the door
    [SerializeField] private float rotationSpeed = 2f;   // Speed of rotation

    private Quaternion closedRotation;

    private GameObject playerCamera;

    private NavMeshObstacle doorObstacle; // Assign in Inspector

    private ShakeEffect shakeEffect;

    [SerializeField]
    private float doorHealth = 10f;

    private float currentHealth;

    private void Start()
    {
        shakeEffect = GetComponent<ShakeEffect>();
        // Save the door's closed rotation
        doorObstacle = GetComponent<NavMeshObstacle>();
        closedRotation = transform.localRotation;
        playerCamera = Camera.main.gameObject;
        currentHealth = doorHealth;
    }

    public void OnInteract()
    {
        if (!isLocked)
        {
            if (IsOpen)
            {
                AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.CloseDoor, this.transform.position);
                CloseDoor(rotationSpeed);
            }
            else
            {
                AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.OpenDoor, this.transform.position);
                OpenDoor(rotationSpeed, playerCamera);
            }
        }
        else
        {
            Event.OnTryToUnlockDoor?.Invoke(this);
        }
    }
    private void OpenDoor(float speed, GameObject user)
    {
        if (!IsOpen)
        {
            StartCoroutine(RotateDoor(CheckDirection(user), speed));
            OnOpen.Invoke();
            IsOpen = true;
        }
    }

    private void CloseDoor(float speed)
    {
        if (IsOpen)
        {
            StartCoroutine(RotateDoor(closedRotation, speed));
            OnClose.Invoke();
            IsOpen = false;
        }
    }

    private Quaternion CheckDirection(GameObject user)
    {
        // Open the door forward relative to the player
        Vector3 doorToPlayer = user.transform.position - transform.position;
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

    public void TakeDamage(float damage, GameObject user)
    {

        if (!IsOpen)
        {
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.TenseLockedDoor, transform.position);
            shakeEffect.ShakeObject();
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            //Play strong openning sound
            OpenDoor(10f, user);
            currentHealth = doorHealth;
        }
    }    

    public void LockedDoor()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.EasyLockedDoor, transform.position);
        shakeEffect.ShakeObject();
        OnInteractLocked.Invoke();
    }

    public void UnlockDoor()
    {
        //Apply sound Here
        OnUnlock.Invoke();
        isLocked = false;
        OpenDoor(rotationSpeed, playerCamera);
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

        if (IsOpen)
        {
            doorObstacle.carving = false;
        }
        else
        {
            doorObstacle.carving = true;
        }
    }

    public void OnTriggerOpen(float speed)
    {
        OpenDoor(speed, playerCamera);
        //Think about opening sounds dependant on the speed
    }

    public void OnTriggerClose(float speed)
    {
        CloseDoor(speed);
        //Think about opening sounds dependant on the speed

    }
    
    public void LockTheDoor(bool islockedDoor)
    {
        isLocked = islockedDoor;
        //Apply sound here
    }

}
