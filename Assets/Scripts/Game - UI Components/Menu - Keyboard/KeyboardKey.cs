using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class KeyboardKey : SelectableElement
{
    [SerializeField] private Char _targetKey;
    [SerializeField] private TMP_Text _keyText;

    private float _jumpHeight => _keyboardController.KeyboardSettings.JumpHeight;
    private float _jumpDuration => _keyboardController.KeyboardSettings.JumpDuration;
    private AnimationCurve _jumpAnimationCurve => _keyboardController.KeyboardSettings.JumpAnimationCurve;

    private KeyControl _targetKeyControl;
    private Image _img;
    private KeyboardController _keyboardController;
    private Tween _scaleTween;

    public void Initialize(KeyboardController keyboardController, Char targetChar)
    {
        _img = GetComponent<Image>();
        _keyText = GetComponentInChildren<TMP_Text>();

        _keyboardController = keyboardController;
        _targetKey = targetChar;
        _targetKeyControl = KeyboardUtils.GetKeyControlFromChar(_targetKey);
        _keyText.text = _targetKey.ToString();


        _keyboardController.OnKeyPressed += OnKeyPressed;
    }

    public void Uninitialize()
    {
        _keyboardController.OnKeyPressed -= OnKeyPressed;
    }

    public void OnKeyPressed(Char character)
    {
        if (character.NormalizeChar() == _targetKey.NormalizeChar())
        {
            var copy = Copy();
            Vector3 targetPosition = _keyboardController.KeyReferenceTarget.transform.position;
            Vector3 downPosition = new Vector3(transform.position.x, _keyboardController.KeyReferenceTarget.transform.position.y, transform.position.z);
            float distance = transform.position.x - targetPosition.x;
            downPosition.x += -distance / 3;

            //copy.GetComponent<Image>().enabled = false;

            copy.transform.DOJump(downPosition, _jumpHeight, 1, _jumpDuration).SetEase(_jumpAnimationCurve).OnComplete(() =>
            {
                copy.GetComponent<Image>().DOFade(0, _jumpDuration).SetDelay(0.2f);
                copy.transform.DOJump(Vector3.Lerp(targetPosition, downPosition, 1 / 2f), _jumpHeight * 2 / 3, 1, _jumpDuration)
                   .SetEase(_jumpAnimationCurve).OnComplete(() =>
                   {
                       copy.transform.DOJump(targetPosition, _jumpHeight * 1 / 3, 1, _jumpDuration)
                        .SetEase(_jumpAnimationCurve).OnComplete(() =>
                        {
                            //copy.GetComponent<CanvasGroup>().DOFade(0, 0.2f).OnComplete(() =>
                            //{
                            //});
                            Destroy(copy);
                            _keyboardController.UpdateTextComponent();
                        });
                   });
            });

            transform.localScale = Vector3.one; // Reset scale before tweening  
            _scaleTween?.Kill();
            _scaleTween = transform.DOPunchScale(Vector3.one * .2f, .5f);
        }
    }

    private GameObject Copy()
    {
        var instance = Instantiate(gameObject, transform.parent);
        instance.transform.SetAsLastSibling();
        instance.transform.position = transform.position;
        Destroy(instance.GetComponent<KeyboardKey>());
        return instance;
    }

    private void FixedUpdate()
    {
        //If we arent using a keyboard we do not want the keycap to respond to keyboard input
        if (ReflectCurrentInput.CurrentInputType != InputType.Keyboard)
            return;


        if (_targetKeyControl != null)
        {
            if (_targetKeyControl.isPressed)
                PressDown();
            else
                PressUp();
        }
    }

    //test
    private void PressDown()
    {
        _img.color = Color.black; // Change color to indicate key press
        _keyText.color = Color.white; // Change text color to white when pressed
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(Vector3.one * .9f, 0.2f).SetEase(Ease.OutBack);

    }

    private void PressUp()
    {
        _img.color = Color.white; // Reset color when key is released
        _keyText.color = Color.black; // Reset text color to black when released
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }


    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        PressDown();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        PressUp();
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        //OnKeyPressed(_targetKey);
        _keyboardController.OnKeyPress(_targetKey);
    }
}
