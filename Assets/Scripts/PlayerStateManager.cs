using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStateManager : MonoBehaviour
{
    #region Singleton
    public static PlayerStateManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private PlayerState _defaultPlayerState;

    [Header("Controllers and Managers")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private CursorController _cursorController;
    [SerializeField] private MenuManager _menuManager;
    [SerializeField] EventSystem _canvasEventSystem;

    private PlayerState _currentPlayerState;
    public PlayerState CurrentPlayerState => _currentPlayerState;

    private void Start()
    {
        SelectPlayerState(_defaultPlayerState);
    }

    public void SelectPlayerState(PlayerState playerState)
    {
        _currentPlayerState = playerState;

        switch (playerState)
        {
            //default state
            case PlayerState.WalkingAround:
                _cameraController.BlockCamera(this, false);
                _movementController.BlockMovement(this, false);
                _cursorController.SetCursorState(CursorStates.Locked);
                _menuManager.BlockMenu(this, false);
                break;
            //if we are using a controller or keyboard we want to hide the mouse cursor
            case PlayerState.InUIWithControllerOrKeyboard:
                _cameraController.BlockCamera(this, true);
                _movementController.BlockMovement(this, true);
                _cursorController.SetCursorState(CursorStates.Locked);
                _menuManager.BlockMenu(this, false);
                break;
            //if we are using a mouse we want to show the mouse cursor
            case PlayerState.InUIWithMouse:
                _cameraController.BlockCamera(this, true);
                _movementController.BlockMovement(this, true);
                _cursorController.SetCursorState(CursorStates.UI);
                _menuManager.BlockMenu(this, false);
                break;
            case PlayerState.InDialogue:
                _cameraController.BlockCamera(this, true);
                _movementController.BlockMovement(this, true);
                _cursorController.SetCursorState(CursorStates.UI);
                _menuManager.BlockMenu(this, true);
                break;
            case PlayerState.Fishing:
                _cameraController.BlockCamera(this, true);
                _movementController.BlockMovement(this, true);
                _cursorController.SetCursorState(CursorStates.Locked);
                _menuManager.BlockMenu(this, true);
                break;
            //possible infinite loop but should never be the case
            default:
                SelectPlayerState(_defaultPlayerState);
                break;

        }
    }
}

public enum PlayerState
{
    WalkingAround,
    InDialogue,
    Fishing,
    InUIWithControllerOrKeyboard,
    InUIWithMouse
}
