using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimateImageColorBySelection : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private TMP_Text _textElement;
    [SerializeField] private Image _imageElement;

    [Header("Color Settings")]
    [SerializeField] private float _transitionDuration = 0.2f;
    [SerializeField] private Color _targetImageColor;
    [SerializeField] private Color _defaultImageColor;
    [SerializeField] private Color _targetTextColor;
    [SerializeField] private Color _defaultTextColor;

    private Tween _imageColorTween;
    private Tween _textColorTween;

    /// <summary>
    /// Called by OnBroadcast message from SelectableElement when selecting a button
    /// </summary>
    public void OnSelected()
    {
        _imageColorTween?.Kill();
        _imageColorTween = _imageElement.DOColor(_targetImageColor, _transitionDuration);

        _textColorTween?.Kill();
        _textColorTween = _textElement.DOColor(_targetTextColor, _transitionDuration);
    }

    /// <summary>
    /// Called by OnBroadcast message from SelectableElement when deselecting a button
    /// </summary>
    public void OnDeselected()
    {
        _imageColorTween?.Kill();
        _imageColorTween = _imageElement.DOColor(_defaultImageColor, _transitionDuration);

        _textColorTween?.Kill();
        _textColorTween = _textElement.DOColor(_defaultTextColor, _transitionDuration);
    }
}
