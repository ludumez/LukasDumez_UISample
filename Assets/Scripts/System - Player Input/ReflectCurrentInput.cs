using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReflectCurrentInput : MonoBehaviour
{
    [SerializeField] InputActionAsset _actionMap;

    public static InputType CurrentInputType;
    public static Action<InputType> OnInputTypeChanged;

    private void OnEnable()
    {
        foreach (var map in _actionMap.actionMaps)
        {
            foreach (var action in map.actions)
            {
                action.performed += OnActionPerformed;
                action.Enable();
            }
        }
    }

    private void OnDisable()
    {
        foreach (var map in _actionMap.actionMaps)
        {
            foreach (var action in map.actions)
            {
                action.performed -= OnActionPerformed;
                action.Disable();
            }
        }
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        var device = context.control.device;
        var lastInputDevice = InputType.Mouse; // Default to Mouse if no device is detected

        switch (context.control.device)
        {
            case Keyboard:
                lastInputDevice = InputType.Keyboard;
                break;
            case Gamepad:
                lastInputDevice = InputType.Gamepad;
                break;
            case Mouse:
                lastInputDevice = InputType.Mouse;
                break;
            default:
                Debug.LogWarning($"Unknown input device: {device.displayName}");
                break;
        }

        //If we have changed the input type we invoke the event to notify any ui that needs to change its behaviour
        //(currently for the menu controller to hide the mouse cursor when using a keyboard or gamepad)
        if (lastInputDevice != CurrentInputType)
        {
            CurrentInputType = lastInputDevice;
            OnInputTypeChanged?.Invoke(lastInputDevice);
        }

    }
}


public enum InputType
{
    Mouse,
    Keyboard,
    Gamepad,
}
