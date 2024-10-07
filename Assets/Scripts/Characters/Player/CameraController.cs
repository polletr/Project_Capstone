using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _camera;
    [SerializeField] float cameraSensitivity = 200.0f;
    [SerializeField] float cameraAcceleration = 5.0f;

    float rotation_x_axis;
    float rotation_y_axis;

    float inputY;
    float inputX;

    private void Update()
    {
        rotation_x_axis += inputY * cameraSensitivity * Time.deltaTime;
        rotation_y_axis += inputX * cameraSensitivity * Time.deltaTime;
        rotation_x_axis = Mathf.Clamp(rotation_x_axis, -90f, 90f);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, rotation_y_axis, 0), cameraAcceleration * Time.deltaTime);
        _camera.localRotation = Quaternion.Euler(-rotation_x_axis, 0, 0);
    }

    public void HandleMouseInput(Vector2 input)
    {
        inputY = input.y;
        inputX = input.x;
    }

    public void HandleGamepadInput(Vector2 input)
    {

    }

}
