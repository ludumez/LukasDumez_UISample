using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private CursorStates _defaultState;


    public void SetCursorState(CursorStates cursorState)
    {
        switch (cursorState)
        {
            case CursorStates.Locked:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case CursorStates.UI:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                break;
        }
    }
}

public enum CursorStates
{
    Locked,
    UI
}
