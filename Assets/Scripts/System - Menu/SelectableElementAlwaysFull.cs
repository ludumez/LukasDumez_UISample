using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableElementAlwaysFull : SelectableElement
{
    //A selectable element that always NEEDS to be filled with an item

    [Header("Element visuals")]
    [SerializeField] private TMP_Text _elementNameText;
    [SerializeField] private TMP_Text _elementDescriptionText;
    [SerializeField] private Image _elementIcon;

    [Header("Selection Feedback")]
    [SerializeField] private Image _elementIconHolder;
    [SerializeField] private Sprite _defaultIconHolderSprite;
    [SerializeField] private Sprite _selectedIconHolderSprite;

    private Action _onSelectedAction;
    private HookSelectionManager _hookSelectionManager;
    private Tween _scaleTween;
    private MenuController _menuController;

    //Called by the unity event input system
    public override void OnSelect(BaseEventData eventData)
    {
        if (eventData.currentInputModule.input.GetMouseButtonDown(0))
            return;

        base.OnSelect(eventData);
        //Don't use select behaviour when using a mouse

        _onSelectedAction?.Invoke();
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        _hookSelectionManager.SelectItem(_currentElement as Item);
        DisplaySubmit();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        EventSystem.current.SetSelectedGameObject(null);
    }


    //To force selection on pointer down since pointer click might not trigger when the element is moving when selected
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnSubmit(eventData);
    }

    public override void SetupElement()
    {
        base.SetupElement();

        if (_elementNameText != null)
            _elementNameText.text = _currentElement.ElementName;
        if (_elementDescriptionText != null)
            _elementDescriptionText.text = _currentElement.ElementDespcription;
        if (_elementIcon != null)
        {
            _elementIcon.sprite = _currentElement.ElementSprite;
            _elementIcon.enabled = _currentElement.ElementSprite != null;
        }
    }

    public void Init(Action OnSelected, HookSelectionManager hookSelectionManager, MenuController menuController)
    {
        _onSelectedAction = OnSelected;
        _hookSelectionManager = hookSelectionManager;
        _menuController = menuController;


        OnItemSelected(_hookSelectionManager.CurrentHookItem);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        HookSelectionManager.OnHookSelected += OnItemSelected;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        CleanTween();
        HookSelectionManager.OnHookSelected -= OnItemSelected;
    }

    private void OnItemSelected(Item item)
    {
        if (item as iDisplayeableElement == _currentElement)
            DisplaySelection();
        else
            DisplayDeselection();
    }

    void DisplaySelection()
    {
        _elementIconHolder.sprite = _selectedIconHolderSprite;
    }

    void DisplayDeselection()
    {
        _elementIconHolder.sprite = _defaultIconHolderSprite;
    }

    private void DisplaySubmit()
    {
        CleanTween();
        _scaleTween = _elementIcon.transform.DOPunchScale(Vector3.one * .2f, .5f).OnComplete(() => _menuController.NavigateToDefaultMenu()); ;
    }

    private void CleanTween()
    {
        _scaleTween?.Kill();
    }

}
