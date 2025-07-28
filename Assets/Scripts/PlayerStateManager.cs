using UnityEngine;

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
    private PlayerState _currentPlayerState;


    [SerializeField] private InteractionManager _interactionManager;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private CursorController _cursorController;
    [SerializeField] private MenuManager _menuManager;

    private void Start()
    {
        SelectPlayerState(_defaultPlayerState);
    }

    public void SelectPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.WalkingAround:
                _cameraController.BlockCamera(this, false);
                _movementController.BlockMovement(this, false);
                _cursorController.SetCursorState(CursorStates.Locked);
                _interactionManager.BlockInteractions(this, false);
                _menuManager.BlockMenu(this, false);
                break;
            case PlayerState.InUI:
                _cameraController.BlockCamera(this, true);
                _movementController.BlockMovement(this, true);
                _cursorController.SetCursorState(CursorStates.UI);
                _interactionManager.BlockInteractions(this, true);
                _menuManager.BlockMenu(this, true);
                break;
            case PlayerState.InDialogue:
                _cameraController.BlockCamera(this, true);
                _movementController.BlockMovement(this, true);
                _cursorController.SetCursorState(CursorStates.UI);
                _interactionManager.BlockInteractions(this, true);
                _menuManager.BlockMenu(this, true);
                break;
            case PlayerState.Fishing:
                _cameraController.BlockCamera(this, true);
                _movementController.BlockMovement(this, true);
                _cursorController.SetCursorState(CursorStates.Locked);
                _interactionManager.BlockInteractions(this, true);
                _menuManager.BlockMenu(this, true);
                break;

        }
    }
}

public enum PlayerState
{
    WalkingAround,
    InUI,
    InDialogue,
    Fishing
}
