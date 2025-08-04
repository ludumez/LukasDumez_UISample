using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    #region Singleton
    public static MenuController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    #endregion


    [Header("Rect references")]
    [SerializeField] private RectTransform _menuBody;
    [SerializeField] private RectTransform _mainBody;
    [SerializeField] private CanvasGroup _mainBodyCanvasGroup;
    [SerializeField] private GameObject _defaultSelection;

    [Header("Menu Control Settings")]
    [SerializeField] private float _exitInputDuration;

    [Header("Keyboard Controller")]
    [SerializeField] private KeyboardController _keyboardController;
    [SerializeField] private NameSelectionElement _nameSelectionElement;

    [Header("Menu Transition Settings")]
    [SerializeField] private float _transitionDuration;
    [SerializeField] private List<TransitionFrameData> _transitionFrameData = new();
    [SerializeField] private float _previewDuration;
    [SerializeField] private List<TransitionFrameData> _previewFrameData = new();
    [SerializeField] private Transform _transitionBackground;
    [SerializeField] private Transform _transitionForeground;
    [SerializeField] private Transform _persistentBackground;
    [SerializeField] private Transform _nextMenuParent;
    [SerializeField] private Transform _currentMenuParent;
    [SerializeField] private Transform _remainingMenuParent;



    //Menu navigation blocking
    private List<object> _blockedOrigins = new List<object>();

    private InventoryManager _inventoryManager;
    private EventSystem _eventSystem;

    private iInventoryInterface[] _inventories;
    private Menu[] _menus;
    private Menu _currentMenu;
    private int _currentMenuIndex;
    private int _lastNavigationDirection;
    private bool _isMenuOpen;
    private bool _blockInput; //Block input during menu animations and transitions
    private bool _inputPressingDown;
    private Tween _mainBodyTween;
    private Tween _mainBodyGroupTween;

    private Coroutine _transitionSequence;
    private Coroutine _previewSequence;
    private Menu _transitionTargetMenu;
    private Menu _transitionPreivousMenu;

    private PlayerStateManager _playerStateManager;
    public float ExitInputDuration => _exitInputDuration;
    public bool IsOpen => _isMenuOpen;

    private void OnEnable()
    {
        PlayerInput.OnMoveActionUI += Move;
        PlayerInput.OnNavigateMenuActionUI += NavigateMenu;
        PlayerInput.OnLeaveMenuActionUI += LeaveMenu;
    }

    void OnDisable()
    {
        PlayerInput.OnMoveActionUI -= Move;
        PlayerInput.OnNavigateMenuActionUI -= NavigateMenu;
        PlayerInput.OnLeaveMenuActionUI -= LeaveMenu;
    }

    public void Initialize()
    {
        _inventoryManager = InventoryManager.Instance;
        _eventSystem = EventSystem.current;
        _playerStateManager = PlayerStateManager.Instance;

        _inventories = GetComponentsInChildren<iInventoryInterface>(true);
        foreach (var inventory in _inventories)
            inventory.InitInventory(_inventoryManager);

        _menus = GetComponentsInChildren<Menu>(true);
        foreach (var menu in _menus)
        {
            menu.InitMenu(_eventSystem, this);
            menu.MenuClose();
        }

        _keyboardController.InitKeyboard(_eventSystem, this);
        _mainBody.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (_blockInput)
            return;

        if (_isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    public void OpenMenu()
    {
        //Open Sequence
        _menuBody.gameObject.SetActive(true);


        _isMenuOpen = true;
        _currentMenuIndex = 0;
        GoToMenu(_menus[_currentMenuIndex], _currentMenu);


        _mainBodyCanvasGroup.alpha = 0;
        _mainBodyGroupTween?.Kill();
        _mainBodyGroupTween = _mainBodyCanvasGroup.DOFade(1, .3f);

        _mainBody.gameObject.SetActive(true);
        _mainBody.localScale = Vector3.zero;
        _mainBodyTween?.Kill();
        _mainBodyTween = _mainBody.DOScale(1, .5f).SetEase(Ease.OutBack);

        _playerStateManager.SelectPlayerState(PlayerState.InUIWithControllerOrKeyboard);
    }

    public void CloseMenu()
    {

        _currentMenu?.MenuClose();

        _isMenuOpen = false;

        _menuBody.gameObject.SetActive(false);
        _mainBody.gameObject.SetActive(false);
        _currentMenu = null;

        _playerStateManager.SelectPlayerState(PlayerState.WalkingAround);
    }

    private void Move(InputAction.CallbackContext context)
    {
        if (_eventSystem.currentSelectedGameObject == null)
            _currentMenu?.ForceSelection();
    }

    private void NavigateMenu(InputAction.CallbackContext context)
    {
        if (_blockInput)
            return;

        Menu previousSelectedMenu = _currentMenu;
        Menu currentSelectedMenu = _currentMenu;
        if (context.started)
        {
            _inputPressingDown = true;

            _lastNavigationDirection = (int)Mathf.Sign(context.ReadValue<float>());
            _currentMenuIndex += _lastNavigationDirection;
            _currentMenuIndex = (_currentMenuIndex + _menus.Length) % _menus.Length;
            currentSelectedMenu = _menus[_currentMenuIndex];

            StopTransition();
            StopPreview();
            _previewSequence = StartCoroutine(PreviewSequence());
        }

        //Only navigate when releasing the bumpers
        if (!context.canceled || !_inputPressingDown)
            return;

        _inputPressingDown = false;
        GoToMenu(_menus[_currentMenuIndex], previousSelectedMenu);

        IEnumerator PreviewSequence()
        {
            currentSelectedMenu?.transform.SetParent(_nextMenuParent);
            previousSelectedMenu?.transform.SetParent(_currentMenuParent);
            currentSelectedMenu.MenuOpen();

            int frameIndex = 0;
            foreach (var frame in _previewFrameData)
            {
                frame.HideFrame();
            }
            _previewFrameData[frameIndex].ShowFrame();
            while (frameIndex + 1 < _previewFrameData.Count)
            {
                yield return new WaitForSeconds(_previewDuration / (float)_previewFrameData.Count);
                _previewFrameData[frameIndex].HideFrame();
                frameIndex++;
                _previewFrameData[frameIndex].ShowFrame();
                _currentMenu.transform.SetParent(_previewFrameData[frameIndex].backgroundImage.transform);
            }
            _previewSequence = null;
        }
    }

    private void StopPreview()
    {
        if (_previewSequence != null)
        {
            StopCoroutine(_previewSequence);
            foreach (var frame in _previewFrameData)
            {
                frame.HideFrame();
            }
            foreach (var frame in _transitionFrameData)
            {
                frame.HideFrame();
            }
        }
    }

    private void StopTransition()
    {
        if (_transitionSequence != null)
        {
            StopCoroutine(_transitionSequence);
            _transitionPreivousMenu?.transform.SetParent(_remainingMenuParent);
            _transitionTargetMenu?.transform.SetParent(_currentMenuParent);
            _transitionPreivousMenu?.MenuClose();//Only close menu if a menu was open
        }
    }

    private void GoToMenu(Menu targetMenu, Menu previousMenu)
    {
        _currentMenu = targetMenu;
        StopTransition();
        StopPreview();
        if (previousMenu != null)
        {
            _transitionSequence = StartCoroutine(NavigationSequence());
        }
        //Don't animate if we are opening the menu without any menus open
        else
        {
            StopTransition();
            StopPreview();
            _currentMenu.MenuOpen();
            _currentMenu?.transform.SetParent(_currentMenuParent);
        }

        IEnumerator NavigationSequence()
        {

            BlockMenu(this, true);

            _transitionTargetMenu = targetMenu;
            _transitionPreivousMenu = previousMenu;
            _transitionTargetMenu.MenuOpen();
            _transitionTargetMenu?.transform.SetParent(_nextMenuParent);
            _transitionPreivousMenu?.transform.SetParent(_currentMenuParent);

            foreach (var frame in _transitionFrameData)
            {
                frame.HideFrame();
            }
            foreach (var frame in _previewFrameData)
            {
                frame.HideFrame();
            }
            int frameIndex = 0;
            _transitionFrameData[frameIndex].ShowFrame();
            _transitionPreivousMenu?.transform.SetParent(_transitionFrameData[frameIndex].backgroundImage.transform);

            while (frameIndex + 1 < _transitionFrameData.Count)
            {
                yield return new WaitForSeconds(_transitionDuration / (float)_transitionFrameData.Count);
                _transitionFrameData[frameIndex].HideFrame();
                frameIndex++;
                _transitionFrameData[frameIndex].ShowFrame();
                _transitionPreivousMenu?.transform.SetParent(_transitionFrameData[frameIndex].backgroundImage.transform);
            }
            yield return new WaitForSeconds(_transitionDuration / (float)_transitionFrameData.Count);
            _transitionFrameData[frameIndex].HideFrame();


            _transitionPreivousMenu?.transform.SetParent(_remainingMenuParent);
            _transitionTargetMenu?.transform.SetParent(_currentMenuParent);
            _transitionTargetMenu?.MenuOpenFinished();
            _transitionPreivousMenu?.MenuClose();//Only close menu if a menu was open

            BlockMenu(this, false);

            _transitionSequence = null;


        }
    }


    /// <summary>
    /// Called from a menu whenever the on menu close input has been called
    /// </summary>
    public void OnMenuLeft()
    {
        _currentMenuIndex = 0;
        GoToMenu(_menus[_currentMenuIndex], _currentMenu);
    }

    private void LeaveMenu(InputAction.CallbackContext context)
    {
        _currentMenu.LeaveMenu(context);
    }

    public void NavigateToMenu(Menu targetMenu)
    {
        //Obtain the current index in which the target menu is located
        for (int i = 0; i < _menus.Length; i++)
        {
            if (_menus[i] == targetMenu)
            {
                _currentMenuIndex = i;
                break;
            }
        }

        GoToMenu(targetMenu, _currentMenu);
    }

    public void NavigateToDefaultMenu()
    {
        _currentMenuIndex = 0;
        Menu previousMenu = _currentMenu;
        _currentMenu = _menus[_currentMenuIndex];
        GoToMenu(_currentMenu, previousMenu);
    }

    /// <summary>
    /// Block the menu input, only allow input if there are no block origins
    /// Block can be caused by menu transition, animation or when receiving input 
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="blockState"></param>
    public void BlockMenu(object origin, bool blockState)
    {
        if (blockState && !_blockedOrigins.Contains(origin))
        {
            _blockedOrigins.Add(origin);
        }
        else
        {
            _blockedOrigins.Remove(origin);
        }


        if (_blockedOrigins.Count > 0)
        {
            _blockInput = true;
            _mainBodyCanvasGroup.interactable = false;
        }
        else
        {
            _blockInput = false;
            _mainBodyCanvasGroup.interactable = true;
            _eventSystem.SetSelectedGameObject(_defaultSelection);
        }
    }



}

[System.Serializable]
public class TransitionFrameData
{
    public Image backgroundImage;
    public Image forgroundImage;

    public void ShowFrame()
    {
        if (backgroundImage != null)
            backgroundImage.enabled = true;
        if (forgroundImage != null)
            forgroundImage.enabled = true;
    }

    public void HideFrame()
    {
        if (backgroundImage != null)
            backgroundImage.enabled = false;
        if (forgroundImage != null)
            forgroundImage.enabled = false;
    }
}
