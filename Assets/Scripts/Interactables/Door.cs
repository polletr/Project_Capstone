using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Door : Interactable , IUnlockable
{
    [field: SerializeField] public int OpenID { get; private set; }
    public bool IsOpen;
    [field: SerializeField] public bool IsLocked{ get; private set; }
    [SerializeField] private UnityEvent OnOpen;
    [SerializeField] private UnityEvent OnClose;
    [SerializeField] private UnityEvent OnUnlock;
    [SerializeField] private UnityEvent OnInteractLocked;

    [SerializeField] private float rotationAngle = 90f; // Angle to open the door
    [SerializeField] private float rotationSpeed = 2f; // Speed of rotation

    [SerializeField] private float doorHealth = 10f;
    [SerializeField] private float distanceToCheckUIPos = 5f;

    [SerializeField] private Transform frontUIPos;
    [SerializeField] private Transform backUIPos;


    private Quaternion closedRotation;

    private GameObject playerCamera;
    
    private ShakeEffect shakeEffect;

    private float currentHealth;

    public bool Rotating { get; private set; }

    private void Start()
    {
        shakeEffect = GetComponent<ShakeEffect>();
        // Save the door's closed rotation
        closedRotation = transform.localRotation;
        playerCamera = Camera.main.gameObject;
        currentHealth = doorHealth;
    }

    private void OnEnable()
    {
        Event.OnKeyPickup += KeyPickup;
    }

    private void OnDisable()
    {
        Event.OnKeyPickup -= KeyPickup;
    }


    private void Update()
    {
        if (playerCamera == null)
            playerCamera = Camera.main.gameObject;
        if (playerCamera == null || indicatorHandler == null || indicatorHandler.IndicatorUI == null) return;

        var distanceToPlayer = Vector3.Distance(playerCamera.transform.position, transform.position);
        if (distanceToPlayer < distanceToCheckUIPos)
        {
            CheckDirectionUI();
        }
    }

    public override void OnInteract()
    {
        if (!IsLocked)
        {
            if (IsOpen)
            {
                CloseDoor(rotationSpeed);
            }
            else
            {
                OpenDoor(rotationSpeed, playerCamera);
            }
        }
        else
        {
            TryToUnlock();
        }
    }

    public void TryToUnlock()
    {
       Event.OnTryToUnlock?.Invoke(this);
    }
    
    public void Unlock() => OpenDoor();
    
    public void StayLocked() =>LockedDoor();
    
    

    private void OpenDoor(float speed, GameObject user)
    {
        if (!IsOpen)
        {
            if (Rotating)
                StopAllCoroutines();

            IsOpen = true;
            StartCoroutine(RotateDoor(CheckDirection(user), speed));
            OnOpen.Invoke();
        }
    }

    private void CloseDoor(float speed)
    {
        if (IsOpen)
        {
            if (Rotating)
                StopAllCoroutines();
            IsOpen = false;
            StartCoroutine(RotateDoor(closedRotation, speed));
            OnClose.Invoke();
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

    private void CheckDirectionUI()
    {
        // Open the door forward relative to the player
        Vector3 doorToPlayer = playerCamera.transform.position - transform.position;
        Vector3 doorForward = transform.right;

        float dotProduct = Vector3.Dot(doorForward, doorToPlayer);
        if (dotProduct > 0)
            indicatorHandler.IndicatorUI.SetIndicatorPosition(frontUIPos.position);
        else
            indicatorHandler.IndicatorUI.SetIndicatorPosition(backUIPos.position);
    }

    public void TakeDamage(float damage, GameObject user)
    {
        if (!IsOpen)
        {
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.TenseLockedDoor,
                transform.position);
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
        if (indicatorHandler != null && indicatorHandler.IndicatorUI != null)
            indicatorHandler.IndicatorUI.SetLockedIndicator(true);
    }

    public void OpenDoor()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.UnlockDoor, transform.position);
        OnUnlock.Invoke();
        IsLocked = false;
        OpenDoor(rotationSpeed, playerCamera);
    }

    private IEnumerator RotateDoor(Quaternion targetRotation, float speed)
    {
        //if (Rotating) yield break;

        if (IsOpen)
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.OpenDoor,
                this.transform.position);
        else
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.CloseDoor,
                this.transform.position);


        Rotating = true;
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
        
        Rotating = false;
    }

    public void OnOpenDoor(float speed)
    {
        OpenDoor(speed, playerCamera);
        //Think about opening sounds dependant on the speed
    }

    public void OnCloseDoor(float speed = 0.8f)
    {
        CloseDoor(speed);
        //Think about opening sounds dependant on the speed
    }

    public void OnLockOrUnlockDoor(bool islockedDoor)
    {
        IsLocked = islockedDoor;
        if (indicatorHandler != null && indicatorHandler.IndicatorUI != null)
            indicatorHandler.IndicatorUI.SetLockedIndicator(IsLocked);

        if (!IsLocked)
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.UnlockDoor, transform.position);
    }

    private void KeyPickup(int keyID)
    {
        if (keyID == OpenID)
        {
            if (indicatorHandler != null && indicatorHandler.IndicatorUI != null)
                indicatorHandler.IndicatorUI.SetLockedIndicator(false);
        }
    }
}