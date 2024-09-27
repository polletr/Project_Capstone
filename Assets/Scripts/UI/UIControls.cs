using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControls : MonoBehaviour
{
    [SerializeField] private GameObject gamepadControls;
    [SerializeField] private GameObject keyboardControls;

    private void OnEnable()
    {
        //InputDeviceDetector.InputDeviceChanged += OnInputDeviceChanged;
        keyboardControls.SetActive(true);
        gamepadControls.SetActive(false);

    }

    private void OnDisable()
    {
       // InputDeviceDetector.InputDeviceChanged -= OnInputDeviceChanged;
    }

    private void OnInputDeviceChanged(InputDevice device)
    {
        if (device is Gamepad)
        {
            gamepadControls.SetActive(true); 
            keyboardControls.SetActive(false);
        }
        else if (device is Keyboard || device is Mouse)
        {
            keyboardControls.SetActive(true);
            gamepadControls.SetActive(false);
        }
    }
}
