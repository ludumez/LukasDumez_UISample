using DG.Tweening;
using UnityEngine;

public class AnimateScaleBySelection : MonoBehaviour
{
    private Tween _scaleAnimation;

    [SerializeField] private float _targetScale;
    [SerializeField] private float _scaleDuration;

    /// <summary>
    /// Called by OnBroadcast message from SelectableElement when selecting a button
    /// </summary>
    public void OnSelected()
    {
        _scaleAnimation?.Kill();
        _scaleAnimation = transform.DOScale(_targetScale, _scaleDuration);
    }

    /// <summary>
    /// Called by OnBroadcast message from SelectableElement when deselecting a button
    /// </summary>
    public void OnDeselected()
    {
        _scaleAnimation?.Kill();
        _scaleAnimation = transform.DOScale(1, _scaleDuration);
    }
}
