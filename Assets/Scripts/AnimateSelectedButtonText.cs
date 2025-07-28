using TMPro;
using UnityEngine;

public class AnimateSelectedButtonText : MonoBehaviour
{
    [SerializeField] private TextAnimationData _textAnimationData;
    private TMP_Text _textHolder;

    private void Start()
    {
        _textHolder = GetComponent<TMP_Text>();
        _textAnimationData.Initialize(_textHolder);
    }

    //Called by broadcast message from SelectableElement
    public void OnSelected()
    {
        _textAnimationData.Run(true);
    }
    //Called by broadcast message from SelectableElement
    public void OnDeselected()
    {
        _textAnimationData.Stop();
    }
}

[System.Serializable]
public class TextAnimationData
{
    public float Duration;//The total duration of the animation

    private TMP_Text _textHolder;
    private Coroutine _currentTextAnimation;

    public void Initialize(TMP_Text textHolder)
    {
        _textHolder = textHolder;
        _textHolder.maxVisibleCharacters = 0; // Ensure the text starts hidden
    }

    public void Run(bool timeScaleDependent)
    {
        if (_textHolder == null)
            return;


        if (_currentTextAnimation != null)
            _textHolder.StopCoroutine(_currentTextAnimation);

        _textHolder.Animate();
    }

    public void Stop()
    {
        if (_currentTextAnimation != null)
            _textHolder.StopCoroutine(_currentTextAnimation);

        _textHolder.maxVisibleCharacters = 0; // Ensure text remains hidden after stopping the animation
    }

}
