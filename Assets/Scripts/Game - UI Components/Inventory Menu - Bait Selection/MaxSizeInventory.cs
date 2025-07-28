using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MaxSizeInventory : Menu, iInventoryInterface
{
    /// <summary>
    /// A max size inventory is an inventory with a given size.
    /// The inventory will always fill out all rows of the generated inventory.
    /// </summary>


    #region Dependencies
    private InventoryManager _inventoryManager;
    private BaitSelectionManager _baitSelectionManager;

    public InventoryManager InventoryManager => _inventoryManager;
    public MenuController MenuController => _menuController;
    #endregion


    //Grid layout references
    [Header("Grid Layout")]
    [SerializeField] private GridLayoutGroup _gridLayourGroup;
    [SerializeField] private RectTransform _prefabElementsHolder;
    [SerializeField] private SelectableElementInspector _elementInspector;
    [SerializeField] private SelectableElementEmpty _inventoryElementPrefab;

    //Slider references
    [Header("Slider values")]
    [SerializeField] private Scrollbar _verticalScrollBar;
    [SerializeField] private float _automaticScroolSpeed;
    [SerializeField] private AnimationCurve _sliderMovementCurve;

    [Header("Inventory Size Text")]
    [SerializeField] private TMP_Text _inventorySizeText;

    [Header("Exit Indication References")]
    [SerializeField] private Image _exitImage;
    [SerializeField] private InventoryExitButton _exitButton;

    //For the background we want to generate a grid that is displayed the same way as our items
    //This grid is supposed to "tile" outside of our grid, as such we generate extra tiles
    [Header("Grid Background")]
    [SerializeField] private RectTransform _gridBackgroundHolder;
    [SerializeField] private GameObject _gridBackgroundPrefab;



    private List<SelectableElement> _generatedElements = new();
    private float _rowCount;
    private int _collumCount;
    private Coroutine _sliderMovementRoutine;
    private Coroutine _fillSequence;
    private Coroutine _emptySequence;
    private Coroutine _changeMenuSequence;
    private Tween _scaleTween;
    private float _currentsliderTarget;
    private int _targetInventorySize;
    private bool _stopInteraction;//Disable any animation from input 

    //Helpers
    private int _spacing => (int)_gridLayourGroup.spacing.x;
    private float _topHeight => -_gridLayourGroup.cellSize.y / 2;
    private float _bottomHeight => -(_rowCount * _spacing + _rowCount * _gridLayourGroup.cellSize.y - (_gridLayourGroup.cellSize.y / 2));

    public void InitInventory(InventoryManager inventoryManager)
    {
        _inventoryManager = inventoryManager;
        _baitSelectionManager = BaitSelectionManager.Instance;
    }

    public override void InitMenu(EventSystem eventSystem, MenuController menuController)
    {
        base.InitMenu(eventSystem, menuController);
        _exitButton.Init(_menuController);
    }

    public override void MenuClose()
    {
        base.MenuClose();
        gameObject.SetActive(false);

        //Cleanup
        _sliderMovementRoutine = null;
        _generatedElements.Clear();
    }

    public override void MenuOpen()
    {
        base.MenuOpen();
        gameObject.SetActive(true);

        GenerateInventory();
        GenerateBackground();
        GenerateNavigation();
        SetupInventoryWeight();

        //Force slider to be at the start when loading inventory
        StopAllCoroutines();
        _exitImage.fillAmount = 0;
        _stopInteraction = false;
        _verticalScrollBar.onValueChanged.Invoke(1);

        //we delay an initial selection for one frame to allow the ui to be rebuilt properly 
        //rebuild by the unity layout builder 
        StartCoroutine(DelayedSelection());
        IEnumerator DelayedSelection()
        {
            yield return new WaitForEndOfFrame();
            _eventSystem.SetSelectedGameObject(_generatedElements.First().gameObject);
        }
    }

    /// <summary>
    /// Method to generate the current UI elements.
    /// This method instantiates and initializes all the selectable elements inside the inventory.
    /// This method generates the required navigation used by the event system to move around the inventory
    /// </summary>
    private void GenerateInventory()
    {
        //Clean previous instance of inventory
        foreach (Transform child in _prefabElementsHolder)
        {
            //don't destroy our background holder
            if (child == _gridBackgroundHolder.transform)
                continue;

            Destroy(child.gameObject);
        }
        _generatedElements.Clear();

        //We need to calculate the rows and collums the inventory is going to generate
        float elementSize = (_gridLayourGroup.cellSize.x + _gridLayourGroup.spacing.x);
        _collumCount = Mathf.FloorToInt((_prefabElementsHolder.parent.GetComponent<RectTransform>().rect.width - _gridLayourGroup.padding.left - _gridLayourGroup.padding.right) / elementSize);

        //always fill the last element in an inventory to show the inventory box
        _rowCount = Mathf.CeilToInt((_inventoryManager.MaxInventorySize) / (float)_collumCount);
        _targetInventorySize = Mathf.CeilToInt(_rowCount * _collumCount);

        for (int i = 0; i < _targetInventorySize; i++)
        {
            SelectableElementEmpty element = Instantiate(_inventoryElementPrefab, _prefabElementsHolder);
            bool isInteractable = i + 1 <= _inventoryManager.MaxInventorySize;

            element.Init(() => NavigateInventoryOnSelection(element.GetComponent<RectTransform>()), isInteractable, _baitSelectionManager, _menuController);
            element.Init(_elementInspector);
            _generatedElements.Add(element);

            //Only interact with elements that are fillable by the inventory
            if (!isInteractable)
                element.interactable = false;
        }

        //Match the inventory items with the UI elements and initalize the UI elements with items 
        int index = 0;
        foreach (var item in _inventoryManager.GetItemsOfType<ItemBait>())
        {
            _generatedElements[index].InitItem(item.Key);
            index++;
        }
    }

    private void GenerateBackground()
    {
        float extraPadding = _gridLayourGroup.cellSize.x + _gridLayourGroup.spacing.x;
        _gridBackgroundHolder.sizeDelta = new Vector2(extraPadding * 2, extraPadding * 2);
        _gridBackgroundHolder.anchoredPosition = new Vector2(0, extraPadding);
        int targetCellCount = Mathf.RoundToInt((_collumCount + 2) * (_rowCount + 2));
        foreach (Transform child in _gridBackgroundHolder)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < targetCellCount; i++)
        {
            Instantiate(_gridBackgroundPrefab, _gridBackgroundHolder);
        }
    }

    private void GenerateNavigation()
    {
        //Generate navigation were the event system wraps around the grid when reaching the left and right edge of the grid

        for (int i = 0; i < _generatedElements.Count; i++)
        {
            Selectable selectable = _generatedElements[i];
            var nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;

            if (i != 0)
            {
                Selectable leftSelectableElement = _generatedElements[i - 1];
                if (leftSelectableElement.interactable)
                    nav.selectOnLeft = leftSelectableElement;
            }

            if (i != _generatedElements.Count - 1)
            {
                Selectable rightSelectableElement = _generatedElements[i + 1];
                if (rightSelectableElement.interactable)
                    nav.selectOnRight = rightSelectableElement;
            }

            if (i >= _collumCount)
            {
                Selectable upperSelectableElement = _generatedElements[i - _collumCount];
                if (upperSelectableElement.interactable)
                    nav.selectOnUp = upperSelectableElement;
            }

            if (i < _targetInventorySize - _collumCount)
            {
                Selectable lowerSelectableElement = _generatedElements[i + _collumCount];
                if (lowerSelectableElement.interactable)
                    nav.selectOnDown = lowerSelectableElement;
            }
            selectable.navigation = nav;
        }
    }

    /// <summary>
    /// Display the current amount of items the inventory is displaying in relation to the max amount of items
    /// the inventory allows
    /// </summary>
    private void SetupInventoryWeight()
    {
        string currentInventoryWeight = $"{_inventoryManager.ItemList.Count}/{_inventoryManager.MaxInventorySize}";
        _inventorySizeText.SetText(currentInventoryWeight);
    }

    /// <summary>
    /// Move the slider around when the player is selecting an element in the inventory without using a mouse.
    /// </summary>
    /// <param name="selected">Selectable Element that has been selected. Called OnSelection. </param>
    private void NavigateInventoryOnSelection(RectTransform selected)
    {
        float targetHeight = Mathf.Clamp(selected.anchoredPosition.y, _bottomHeight, _topHeight);
        float proportionalHeight = (targetHeight - _bottomHeight) / (_topHeight - _bottomHeight);

        if (_sliderMovementRoutine != null)
            StopCoroutine(_sliderMovementRoutine);

        _sliderMovementRoutine = StartCoroutine(MoveSequence(proportionalHeight));
        IEnumerator MoveSequence(float targetValue)
        {
            float startValue = _verticalScrollBar.value;
            float targetSliderValue = targetValue;
            //time bound and not speed bound to allow control by animation curve
            //time bound allows for easier end check to properly cleanup coroutine
            //speed = distance/time  => time = distance/speed
            float duration = (Mathf.Abs(_verticalScrollBar.value - targetValue)) / _automaticScroolSpeed;
            float t = 0;

            while (t < duration)
            {
                yield return new WaitForFixedUpdate();
                _currentsliderTarget = Mathf.Lerp(startValue, targetSliderValue, _sliderMovementCurve.Evaluate(t / duration));

                _verticalScrollBar.onValueChanged.Invoke(Mathf.Lerp(_verticalScrollBar.value, targetSliderValue, _sliderMovementCurve.Evaluate(t / duration)));
                t += Time.fixedDeltaTime;
            }
            _sliderMovementRoutine = null;
        }
    }

    /// <summary>
    /// Select an element in the input system if no element has been selected
    /// Called from Move from the input system
    /// </summary>
    public override void ForceSelection()
    {
        base.ForceSelection();
        if (_generatedElements.Count > 0)
            _eventSystem.SetSelectedGameObject(_generatedElements.First().gameObject);
    }

    public override void LeaveMenu(InputAction.CallbackContext context)
    {
        base.LeaveMenu(context);
        if (_stopInteraction)
            return;

        if (context.started)
        {
            if (_emptySequence != null)
                StopCoroutine(_emptySequence);
            if (_fillSequence != null)
                StopCoroutine(_fillSequence);
            _fillSequence = StartCoroutine(FillSequence());
        }
        else if (context.canceled)
        {
            if (_emptySequence != null)
                StopCoroutine(_emptySequence);
            if (_fillSequence != null)
                StopCoroutine(_fillSequence);
            _emptySequence = StartCoroutine(EmptySequence());
        }

        IEnumerator FillSequence()
        {
            while (_exitImage.fillAmount < 1)
            {
                yield return new WaitForFixedUpdate();
                _exitImage.fillAmount += _menuController.ExitInputDuration * Time.fixedDeltaTime;
            }

            if (_changeMenuSequence != null)
                StopCoroutine(_changeMenuSequence);
            _changeMenuSequence = StartCoroutine(ChangeMenuSequence());
            _fillSequence = null;
        }

        IEnumerator EmptySequence()
        {
            while (_exitImage.fillAmount > 0)
            {
                yield return new WaitForFixedUpdate();
                _exitImage.fillAmount -= _menuController.ExitInputDuration * Time.fixedDeltaTime;
            }
            _emptySequence = null;
        }

        IEnumerator ChangeMenuSequence()
        {
            _stopInteraction = true;

            _scaleTween?.Kill();
            _scaleTween = _exitImage.transform.DOPunchScale(Vector3.one * .2f, .2f);
            yield return new WaitForSeconds(.3f);
            _menuController.OnMenuLeft();
        }
    }

    private void OnDisable()
    {
        _emptySequence = null;
        _fillSequence = null;
        _scaleTween?.Kill();
        _scaleTween = null;
        _changeMenuSequence = null;

        StopAllCoroutines();
    }
}
