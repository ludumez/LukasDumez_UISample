using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BaitSelectionSelectableElementMenuTrigger : SelectableElement
{
    //Class for button to control the opening and selection of the bait selection menu
    [SerializeField] private Image _itemImage;

    private MenuController _menuController;
    private BaitSelectionManager _baitSelectionManager;
    private MaxSizeInventory _maxSizeInventory;

    private Tween _moveTween;
    private Tween _roationTween;

    public void Init(MenuController menuController, BaitSelectionManager baitSelectionManager, MaxSizeInventory maxSizeInventory)
    {
        _menuController = menuController;
        _baitSelectionManager = baitSelectionManager;
        _maxSizeInventory = maxSizeInventory;
    }

    public override void SetupElement()
    {
        base.SetupElement();

        if (_currentElement != null)
        {
            _itemImage.sprite = _currentElement.ElementSprite;
            _itemImage.enabled = true;
        }
        else
        {
            _itemImage.sprite = null;
            _itemImage.enabled = false;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        _elementInspector.DisplayElement(_baitSelectionManager.CurrentSelectedBait, true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _elementInspector.DisplayElement(null, false);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        _elementInspector.DisplayElement(_baitSelectionManager.CurrentSelectedBait, true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _elementInspector.DisplayElement(null, false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnSubmit(eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        if (_baitSelectionManager.CurrentSelectedBait != null)
        {
            _moveTween = _itemImage.transform.DOLocalJump(new Vector3(0, 0, 0), 10, 1, .2f);
            _roationTween = _itemImage.transform.DOPunchRotation(new Vector3(0, 0, 10), .6f).OnComplete
                (
                () => _menuController.NavigateToMenu(_maxSizeInventory)
                );
        }
        else
        {
            _menuController.NavigateToMenu(_maxSizeInventory);

        }
    }

}
