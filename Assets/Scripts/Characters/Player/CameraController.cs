using UnityEngine;
using System.Collections;
using System.Data.SqlTypes;
using Unity.Mathematics;

public class CameraController : MonoBehaviour
{   
    [SerializeField] private float rotationSpeed = 2f;
    [Header("Reference")] 
    [SerializeField] private Transform camera;

    private Quaternion? startRotationHead;
    private Quaternion? startRotationCamera;

    private IEnumerator RotateToLookAt(Transform target)
    {
        // Calculate the direction to the target and the target rotation for the head (X axis only)
        Vector3 directionToTarget = target.position - camera.transform.position;
       // directionToTarget.y = 0; // Remove Y component to keep the head level
        
        Quaternion targetRotationHead = Quaternion.LookRotation(directionToTarget);
        Quaternion lookAt = camera.transform.rotation;
        for (int i = 0; i < 100; i++)
        {

            camera.transform.rotation = Quaternion.Slerp( lookAt,targetRotationHead, i / 99f);
            yield return null;
        }
        
        //transform.rotation = Quaternion.Euler(0, camera.transform.rotation.y, 0);
        Debug.Log($"Camera local rotation is {camera.transform.localRotation} and the rotation is {camera.transform.rotation}");
        var newEuler = camera.transform.eulerAngles;
        transform.localRotation = Quaternion.Euler(0, newEuler.y, 0);
        camera.transform.localRotation = Quaternion.Euler(newEuler.x, 0, newEuler.z);
        //camera.transform.eulerAngles = Quaternion.Euler(camera.transform) = new Quaternion(camera.transform.localRotation.x, 0, camera.transform.localRotation.z, camera.transform.localRotation.w);
        yield return null;
        Debug.Log($"NEW Camera local rotation is {camera.transform.localRotation} and the rotation is {camera.transform.rotation}");
        // Store initial rotations
        // startRotationHead = transform.localRotation;
        // startRotationCamera = camera.localRotation;
        //
        // // Lerp camera's Y rotation to zero
        // while (Mathf.Abs(camera.localRotation.eulerAngles.x) > 0.01f)
        // {
        //     float newCameraYRotation = Mathf.LerpAngle(camera.localRotation.eulerAngles.x, 0, rotationSpeed * Time.deltaTime);
        //     camera.localRotation = Quaternion.Euler(newCameraYRotation,camera.localRotation.eulerAngles.y, camera.localRotation.eulerAngles.z);
        //     yield return null;
        // }
        //
        // // Lerp head's X rotation to look at the target
        // while (Quaternion.Angle(transform.localRotation, targetRotationHead) > 0.01f)
        // {
        //     transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotationHead, rotationSpeed * Time.deltaTime);
        //     yield return null;
        // }

        // Ensure final alignment
        // transform.localRotation = targetRotationHead;
    }

    private IEnumerator ReturnToStart()
    {
        yield return null;
        StopAllCoroutines();
        if (!startRotationHead.HasValue || !startRotationCamera.HasValue) yield break;

        // Lerp head back to its original X rotation
        while (Quaternion.Angle(transform.localRotation, startRotationHead.Value) > 0.01f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotationHead.Value, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Lerp camera's Y rotation back to its original value
        while (Mathf.Abs(camera.localRotation.eulerAngles.y - startRotationCamera.Value.eulerAngles.y) > 0.01f)
        {
            float newCameraYRotation = Mathf.LerpAngle(camera.localRotation.eulerAngles.y, startRotationCamera.Value.eulerAngles.y, rotationSpeed * Time.deltaTime);
            camera.localRotation = Quaternion.Euler(camera.localRotation.eulerAngles.x, newCameraYRotation, camera.localRotation.eulerAngles.z);
            yield return null;
        }

        // Ensure final alignment
        transform.localRotation = startRotationHead.Value;
        camera.localRotation = startRotationCamera.Value;

        startRotationHead = null;
        startRotationCamera = null;
    }

    public void LookAtTarget(Transform target)
    {
        StartCoroutine(RotateToLookAt(target));
    }

    public void RotateBack()
    {
        if (startRotationHead.HasValue && startRotationCamera.HasValue)
        {
            StartCoroutine(ReturnToStart());
        }
    }
}