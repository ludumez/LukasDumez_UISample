using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    //Manager to manage the opening and closing of the menu
    //Opening and closing the menu can come from different points
    //Limiting the opening and closing of the menu can come from different points
    //Selected menu initialization for proper execution order

    [SerializeField] private MenuController _menuController;

    private List<object> _controllerBlockers = new();
    private bool _block;
    private PlayerStateManager _playerStateManager;

    private void OnEnable()
    {
        ReflectCurrentInput.OnInputTypeChanged += OnInputDeviceChanged;
        PlayerInput.OnOpenMenuActionUI += Toggle;
    }

    private void OnDisable()
    {
        ReflectCurrentInput.OnInputTypeChanged -= OnInputDeviceChanged;
        PlayerInput.OnOpenMenuActionUI -= Toggle;
    }

    private void Start()
    {
        _menuController.Initialize();
        _playerStateManager = PlayerStateManager.Instance;
    }

    //We listen to see if we change the input type,
    //if we are using the keyboard or gamepad to navigate we want to hide the mouse cursor
    private void OnInputDeviceChanged(InputType type)
    {

        switch (type)
        {
            case InputType.Keyboard:
                _playerStateManager.SelectPlayerState(PlayerState.InUIWithControllerOrKeyboard);
                break;
            case InputType.Gamepad:
                _playerStateManager.SelectPlayerState(PlayerState.InUIWithControllerOrKeyboard);
                break;
            case InputType.Mouse:
                _playerStateManager.SelectPlayerState(PlayerState.InUIWithMouse);
                break;
            default:
                Debug.LogWarning($"Unknown input type: {type}");
                break;
        }

    }


    public void Toggle(InputAction.CallbackContext ctx)
    {

        if (ctx.started && !_block)
        {
            _menuController.Toggle();
        }
    }

    public void BlockMenu(object origin, bool state)
    {
        if (state && !_controllerBlockers.Contains(origin))
            _controllerBlockers.Add(origin);
        else if (!state && _controllerBlockers.Contains(origin))
            _controllerBlockers.Remove(origin);

        _block = _controllerBlockers.Count > 0;
    }
}
