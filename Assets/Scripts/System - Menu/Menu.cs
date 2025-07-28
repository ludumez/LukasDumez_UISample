using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour, iMenu
{
    protected EventSystem _eventSystem;
    protected MenuController _menuController;
    public EventSystem EventSystem => _eventSystem;


    public virtual void ForceSelection()
    {
    }

    public virtual void InitMenu(EventSystem eventSystem, MenuController menuController)
    {
        _eventSystem = eventSystem;
        _menuController = menuController;
    }

    public virtual void LeaveMenu(InputAction.CallbackContext context)
    {
    }

    public virtual void MenuClose()
    {
    }

    public virtual void MenuOpen()
    {
    }

    //Called after the opening animation has been completed
    //To fix an issue where the player can navigate two menus at the same time
    public virtual void MenuOpenFinished() { }

}
