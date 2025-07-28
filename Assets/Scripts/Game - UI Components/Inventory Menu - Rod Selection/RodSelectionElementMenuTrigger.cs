using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RodSelectionElementMenuTrigger : SelectableElement
{
    //Class for a button to control the opening of the skill tree menu
    [SerializeField] private Image _currentRodIconDisplay;

    private MenuController _menuController;
    private HookSelectionManager _hookSelectionManager;
    private Menu _rodMenu;

    private Tween _scaleTween;
    private Tween _moveTween;
    private Tween _rotationTween;

    public void Init(MenuController menuController, Menu rodMenu, HookSelectionManager hookSelectionManager)
    {
        _menuController = menuController;
        _hookSelectionManager = hookSelectionManager;
        _rodMenu = rodMenu;
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);

        _moveTween?.Kill();
        _rotationTween?.Kill();

        if (_hookSelectionManager.CurrentHookItem != null)
        {
            _moveTween = _currentRodIconDisplay.transform.DOLocalJump(new Vector3(0, 0, 0), 10, 1, .2f);
            _rotationTween = _currentRodIconDisplay.transform.DOPunchRotation(new Vector3(0, 0, 10), .6f).OnComplete
                (
                () => _menuController.NavigateToMenu(_rodMenu)
                );
        }
        else
        {
            _menuController.NavigateToMenu(_rodMenu);
        }
    }

    public override void SetupElement()
    {
        base.SetupElement();

        if (_currentElement != null)
        {
            _currentRodIconDisplay.sprite = _currentElement.ElementSprite;
            _currentRodIconDisplay.enabled = true;
        }
        else
        {
            _currentRodIconDisplay.sprite = null;
            _currentRodIconDisplay.enabled = false;
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnSubmit(eventData);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _moveTween?.Kill();
        _rotationTween?.Kill();
        _scaleTween?.Kill();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        _elementInspector.DisplayElement(_hookSelectionManager.CurrentHookItem, true);
    }
}
