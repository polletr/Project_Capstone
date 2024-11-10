using UnityEngine;
using UnityEngine.InputSystem;

public class ClampedCameraMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;        // Sensitivity of mouse/joystick movement
    public float minVerticalAngle = -45f;        // Minimum angle for vertical rotation (down)
    public float maxVerticalAngle = 45f;         // Maximum angle for vertical rotation (up)
    public float minHorizontalAngle = -30f;      // Minimum angle for horizontal rotation (left)
    public float maxHorizontalAngle = 30f;       // Maximum angle for horizontal rotation (right)

    private float xRotation = 0f;                // Tracks current vertical rotation for clamping
    private float yRotation = 0f;                // Tracks current horizontal rotation for clamping

    [Tooltip("Reference to the look input action from the input map")]
    public InputActionReference lookActionReference;

    private void OnEnable()
    {
        if (lookActionReference != null)
        {
            lookActionReference.action.Enable();  // Enable the action from the input map
        }
    }

    private void OnDisable()
    {
        if (lookActionReference != null)
        {
            lookActionReference.action.Disable();  // Disable the action from the input map
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Lock cursor to center of screen
    }

    void Update()
    {
        // Get the look input from the existing input map
        Vector2 lookInput = lookActionReference.action.ReadValue<Vector2>();

        // Calculate rotation values based on input and sensitivity
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Update and clamp the X rotation (pitch) for vertical movement
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        // Update and clamp the Y rotation (yaw) for horizontal movement
        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, minHorizontalAngle, maxHorizontalAngle);

        // Apply the clamped rotations to the camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}