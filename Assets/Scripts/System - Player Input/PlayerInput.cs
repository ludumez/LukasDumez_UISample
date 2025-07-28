using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputActionMapAsset inputActionMap;

    //UI
    private InputAction moveActionUI;
    private InputAction openMenuActionUI;
    private InputAction navigateMenuActionUI;
    private InputAction leaveMenuActionUI;
    private InputAction backActionUI;
    private InputAction confirmActionUI;

    //Main
    private InputAction moveActionMain;
    private InputAction lookActionMain;
    private InputAction interactActionMain;
    private InputAction jumpActionMain;

    //Static events
    //Make sure to clear them properly when disabling to avoid memory issues

    //UI EVENTS
    public static Action<InputAction.CallbackContext> OnMoveActionUI;
    public static Action<InputAction.CallbackContext> OnOpenMenuActionUI;
    public static Action<InputAction.CallbackContext> OnNavigateMenuActionUI;
    public static Action<InputAction.CallbackContext> OnLeaveMenuActionUI;
    public static Action<InputAction.CallbackContext> OnBackActionUI;
    public static Action<InputAction.CallbackContext> OnConfirmActionUI;

    //Main EVENTS
    public static Action<InputAction.CallbackContext> OnMoveActionMain;
    public static Action<InputAction.CallbackContext> OnLookActionMain;
    public static Action<InputAction.CallbackContext> OnInteractActionMain;
    public static Action<InputAction.CallbackContext> OnJumpActionMain;

    private void Awake()
    {
        inputActionMap = new InputActionMapAsset();

        //UI
        moveActionUI = inputActionMap.UI.Move;
        openMenuActionUI = inputActionMap.UI.OpenMenu;
        navigateMenuActionUI = inputActionMap.UI.MoveMenu;
        leaveMenuActionUI = inputActionMap.UI.ExitMenu;
        backActionUI = inputActionMap.UI.Back;
        confirmActionUI = inputActionMap.UI.Confirm;

        //Main
        moveActionMain = inputActionMap.Movement.Move;
        lookActionMain = inputActionMap.Movement.Look;
        interactActionMain = inputActionMap.Movement.Interact;
        jumpActionMain = inputActionMap.Movement.Jump;
    }

    private void OnEnable()
    {
        //UI
        moveActionUI.started += OnMoveUI;
        moveActionUI.performed += OnMoveUI;
        moveActionUI.canceled += OnMoveUI;

        openMenuActionUI.started += OnOpenMenuUI;
        openMenuActionUI.performed += OnOpenMenuUI;
        openMenuActionUI.canceled += OnOpenMenuUI;


        navigateMenuActionUI.started += OnNavigateMenuUI;
        navigateMenuActionUI.performed += OnNavigateMenuUI;
        navigateMenuActionUI.canceled += OnNavigateMenuUI;

        leaveMenuActionUI.started += OnLeaveMenuUI;
        leaveMenuActionUI.performed += OnLeaveMenuUI;
        leaveMenuActionUI.canceled += OnLeaveMenuUI;

        backActionUI.started += OnBackUI;
        backActionUI.performed += OnBackUI;
        backActionUI.canceled += OnBackUI;

        confirmActionUI.started += OnConfirmUI;
        confirmActionUI.performed += OnConfirmUI;
        confirmActionUI.canceled += OnConfirmUI;

        moveActionUI.Enable();
        openMenuActionUI.Enable();
        navigateMenuActionUI.Enable();
        leaveMenuActionUI.Enable();
        backActionUI.Enable();
        confirmActionUI.Enable();


        //Main
        moveActionMain.started += OnMoveMain;
        moveActionMain.performed += OnMoveMain;
        moveActionMain.canceled += OnMoveMain;

        lookActionMain.started += OnLookMain;
        lookActionMain.performed += OnLookMain;
        lookActionMain.canceled += OnLookMain;

        interactActionMain.started += OnInteractMain;
        interactActionMain.performed += OnInteractMain;
        interactActionMain.canceled += OnInteractMain;

        jumpActionMain.started += OnJumpMain;
        jumpActionMain.performed += OnJumpMain;
        jumpActionMain.canceled += OnJumpMain;

        moveActionMain.Enable();
        lookActionMain.Enable();
        interactActionMain.Enable();
        jumpActionMain.Enable();
    }


    private void OnDisable()
    {
        //UI
        moveActionUI.started -= OnMoveUI;
        moveActionUI.performed -= OnMoveUI;
        moveActionUI.canceled -= OnMoveUI;

        openMenuActionUI.started -= OnOpenMenuUI;
        openMenuActionUI.performed -= OnOpenMenuUI;
        openMenuActionUI.canceled -= OnOpenMenuUI;

        navigateMenuActionUI.started -= OnNavigateMenuUI;
        navigateMenuActionUI.performed -= OnNavigateMenuUI;
        navigateMenuActionUI.canceled -= OnNavigateMenuUI;

        leaveMenuActionUI.started -= OnLeaveMenuUI;
        leaveMenuActionUI.performed -= OnLeaveMenuUI;
        leaveMenuActionUI.canceled -= OnLeaveMenuUI;

        backActionUI.started -= OnBackUI;
        backActionUI.performed -= OnBackUI;
        backActionUI.canceled -= OnBackUI;

        confirmActionUI.started -= OnConfirmUI;
        confirmActionUI.performed -= OnConfirmUI;
        confirmActionUI.canceled -= OnConfirmUI;

        moveActionUI.Disable();
        openMenuActionUI.Disable();
        navigateMenuActionUI.Disable();
        leaveMenuActionUI.Disable();
        backActionUI.Disable();
        confirmActionUI.Disable();


        //Main
        moveActionMain.started -= OnMoveMain;
        moveActionMain.performed -= OnMoveMain;
        moveActionMain.canceled -= OnMoveMain;

        lookActionMain.started -= OnLookMain;
        lookActionMain.performed -= OnLookMain;
        lookActionMain.canceled -= OnLookMain;

        interactActionMain.started -= OnInteractMain;
        interactActionMain.performed -= OnInteractMain;
        interactActionMain.canceled -= OnInteractMain;

        jumpActionMain.started += OnJumpMain;
        jumpActionMain.performed += OnJumpMain;
        jumpActionMain.canceled += OnJumpMain;

        moveActionMain.Disable();
        lookActionMain.Disable();
        interactActionMain.Disable();
        jumpActionMain.Disable();
    }

    private void OnMoveUI(InputAction.CallbackContext callbackContext)
    {
        OnMoveActionUI?.Invoke(callbackContext);
    }

    private void OnOpenMenuUI(InputAction.CallbackContext callbackContext)
    {
        OnOpenMenuActionUI?.Invoke(callbackContext);
    }

    private void OnNavigateMenuUI(InputAction.CallbackContext callbackContext)
    {
        OnNavigateMenuActionUI?.Invoke(callbackContext);
    }

    private void OnLeaveMenuUI(InputAction.CallbackContext callbackContext)
    {
        OnLeaveMenuActionUI?.Invoke(callbackContext);
    }

    private void OnBackUI(InputAction.CallbackContext callbackContext)
    {
        OnBackActionUI?.Invoke(callbackContext);
    }

    private void OnConfirmUI(InputAction.CallbackContext callbackContext)
    {
        OnConfirmActionUI?.Invoke(callbackContext);
    }

    private void OnMoveMain(InputAction.CallbackContext callbackContext)
    {
        OnMoveActionMain?.Invoke(callbackContext);
    }

    private void OnLookMain(InputAction.CallbackContext callbackContext)
    {
        OnLookActionMain?.Invoke(callbackContext);
    }
    private void OnInteractMain(InputAction.CallbackContext callbackContext)
    {
        OnInteractActionMain?.Invoke(callbackContext);
    }
    private void OnJumpMain(InputAction.CallbackContext callbackContext)
    {
        OnJumpActionMain?.Invoke(callbackContext);
    }

}

