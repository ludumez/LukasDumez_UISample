using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTreeSelectableElementMenuTrigger : SelectableElement
{
    //Class for a button to control the opening of the skill tree menu
    [SerializeField] private float _scaleTarget;
    [SerializeField] private float _scaleDuration;

    private MenuController _menuController;
    private Menu _skillMenu;

    private Tween _scaleTween;
    private Tween _moveTween;
    private Tween _rotationTween;
    public void Init(MenuController menuController, Menu skillMenu)
    {
        _menuController = menuController;
        _skillMenu = skillMenu;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GainFocus();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        LoseFocus();
    }
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        GainFocus();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        LoseFocus();
    }


    private void LoseFocus()
    {
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(1, _scaleDuration);
    }

    private void GainFocus()
    {
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(_scaleTarget, _scaleDuration);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);

        _moveTween?.Kill();
        _rotationTween?.Kill();

        _moveTween = transform.DOLocalJump(transform.localPosition, 10, 1, .2f);
        _rotationTween = transform.DOPunchRotation(new Vector3(0, 0, 10), .6f).OnComplete
            (
            () => _menuController.NavigateToMenu(_skillMenu)
            );

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
}
