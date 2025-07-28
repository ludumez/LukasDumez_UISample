using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public interface iMenu
{
    //To keep a reference to all the inventories we want to toggle trough

    public EventSystem EventSystem { get; }
    public virtual void InitMenu(EventSystem eventSystem) { }
    public virtual void MenuOpen() { }
    public virtual void MenuClose() { }

    //Force Selection when we want the player to regain move control trough input
    public virtual void ForceSelection() { }
    public virtual void LeaveMenu(InputAction.CallbackContext context) { }

}
