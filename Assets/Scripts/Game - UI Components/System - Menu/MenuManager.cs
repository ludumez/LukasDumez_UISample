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

    private void OnEnable()
    {
        PlayerInput.OnOpenMenuActionUI += Toggle;
    }

    private void OnDisable()
    {
        PlayerInput.OnOpenMenuActionUI -= Toggle;
    }

    private void Start()
    {
        _menuController.Initialize();
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
