using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SelectableElementEmpty : SelectableElement
{
    //an element that can be selected but doesn't need to have an item assigned to it
    //used to create an inventory with empty slots


    [SerializeField] private Sprite _emptyElementSprite;
    [SerializeField] private Sprite _fullElementSprite;
    [SerializeField] private Sprite _selectedElementSprite;
    [SerializeField] protected Image _itemImage;
    [SerializeField] protected Image _borderImage;

    private BaitSelectionManager _selectionManager;
    private MenuController _menuController;
    private Action _onSelectAction;
    private Tween _moveTween;
    private Tween _roationTween;
    private bool _isFillable;

    public override void SetupElement()
    {
        base.SetupElement();
        _itemImage.enabled = _currentElement != null;
        _itemImage.sprite = _currentElement.ElementSprite;
    }

    public void Init(Action OnSelect, bool isFillable, BaitSelectionManager selectionManager, MenuController menuController)
    {
        _onSelectAction = OnSelect;
        _selectionManager = selectionManager;
        _menuController = menuController;

        _isFillable = isFillable;
        _itemImage.enabled = _currentElement != null;
        _borderImage.sprite = !isFillable ? _emptyElementSprite : _fullElementSprite;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        BaitSelectionManager.OnItemSelected += OnItemSelected;
    }


    //Called by the unity event input system
    public override void OnSelect(BaseEventData eventData)
    {
        //Don't use select behaviour when using a mouse
        if (eventData.currentInputModule.input.GetMouseButtonDown(0))
            return;

        base.OnSelect(eventData);
        DisplaySelection();
        _onSelectAction?.Invoke();
    }


    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        DisplayDeselection();
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        DisplaySubmit();
        if (_currentElement != null)
        {
            _selectionManager.SelectItem(_currentElement as Item);
            _elementInspector.DisplayElement(_currentElement, true);
        }

    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        //base.OnSelect(eventData);
        EventSystem.current.SetSelectedGameObject(null);
        DisplaySelection();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        //base.OnDeselect(eventData);
        DisplayDeselection();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnSubmit(eventData);
    }

    private void DisplaySubmit()
    {
        CleanTween();
        _roationTween = _itemImage.transform.DOPunchRotation(new Vector3(0, 0, 10), .6f);
        _moveTween = _itemImage.transform.DOLocalJump(new Vector3(0, 0, 0), 10, 1, .2f).OnComplete(() => _menuController.NavigateToDefaultMenu());
    }

    private void DisplaySelection()
    {
        if (_currentElement == null)
            _elementInspector.DisplayElement(_selectionManager.CurrentSelectedBait, true);
        else
            _elementInspector.DisplayElement(_currentElement, (object)_selectionManager.CurrentSelectedBait == (object)_currentElement);

        CleanTween();
        _moveTween = _itemImage.transform.DOLocalMoveY(5, .2f);
    }

    private void DisplayDeselection()
    {
        CleanTween();
        _moveTween = _itemImage.transform.DOLocalMoveY(0, .2f);
    }

    private void CleanTween()
    {
        _moveTween?.Kill();
        _roationTween?.Kill();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CleanTween();
        _onSelectAction = null;
        _selectionManager = null;
        BaitSelectionManager.OnItemSelected -= OnItemSelected;
    }

    private void OnItemSelected(Item item)
    {
        if (item as iDisplayeableElement == _currentElement)
            _borderImage.sprite = _selectedElementSprite;
        else
        {
            _borderImage.sprite = !_isFillable ? _emptyElementSprite : _fullElementSprite;
        }
    }

}
