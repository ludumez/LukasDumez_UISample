using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryExitButton : SelectableElement
{
    [SerializeField] private float _scaleSize;
    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _fillDuration;
    [SerializeField] private float _selectionScale;
    [SerializeField] private float _selectionScaleDuration;
    [SerializeField] private Image _exitIconFill;

    private bool _blockInput;
    private MenuController _menuController;

    private Tween _scaleTween;
    private Tween _fillTween;
    private Tween _rotateTween;
    private Tween _jumpTween;

    private Coroutine _exitCouroutine;

    protected override void OnEnable()
    {
        _exitIconFill.fillAmount = 0;
        _blockInput = false;
        transform.localScale = Vector3.one;
    }
    protected override void OnDisable()
    {
        _fillTween?.Kill();
        _scaleTween?.Kill();
        _rotateTween?.Kill();
        _jumpTween?.Kill();
    }

    public void Init(MenuController menuController)
    {
        _menuController = menuController;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (_blockInput)
            return;

        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(_scaleSize, _scaleDuration);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (_blockInput)
            return;

        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(1, _scaleDuration);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (_blockInput)
            return;

        _blockInput = true;

        if (_exitCouroutine != null)
            StopCoroutine(_exitCouroutine);
        _exitCouroutine = StartCoroutine(Sequence());

        IEnumerator Sequence()
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOPunchScale(Vector3.one * _selectionScale, _selectionScaleDuration);
            _rotateTween?.Kill();
            _rotateTween = transform.DOPunchRotation(new Vector3(0, 0, 10), _selectionScaleDuration);
            _jumpTween?.Kill();
            _jumpTween = transform.DOLocalJump(transform.localPosition, 10, 1, _selectionScaleDuration);

            yield return new WaitForSeconds(_selectionScaleDuration);
            yield return new WaitForSeconds(.2f);

            _fillTween?.Kill();
            _fillTween = _exitIconFill.DOFillAmount(1, _fillDuration);
            yield return new WaitForSeconds(_fillDuration + .05f);

            _menuController.NavigateToDefaultMenu();

        }
    }
}
