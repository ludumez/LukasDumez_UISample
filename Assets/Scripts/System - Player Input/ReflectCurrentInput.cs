using UnityEngine;
using UnityEngine.InputSystem;

public class ReflectCurrentInput : MonoBehaviour
{

    [SerializeField] InputActionAsset _actionMap;

    public static InputType CurrentInputType;


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

        switch (context.control.device)
        {
            case Keyboard:
                CurrentInputType = InputType.MouseAndKeyboard;
                break;
            case Gamepad:
                CurrentInputType = InputType.Gamepad;
                break;
            case Mouse:
                CurrentInputType = InputType.MouseAndKeyboard;
                break;
            default:
                Debug.LogWarning($"Unknown input device: {device.displayName}");
                break;
        }
    }
}


public enum InputType
{
    MouseAndKeyboard,
    Gamepad,
}
