using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimatedButton : MonoBehaviour
{
    [SerializeField] private ImageFrameData _buttonImageFrameData;

    private Button _button;
    private void Start()
    {
        _button = GetComponent<Button>();
        _buttonImageFrameData.Initialize(_button);
    }

    //Called by broadcast message from SelectableElement
    public void OnSelected()
    {
        _buttonImageFrameData.Run(true);
    }

    //Called by broadcast message from SelectableElemen
    public void OnDeselected()
    {
        _buttonImageFrameData.Stop();
    }

}

[System.Serializable]
public class ImageFrameData
{
    public float FrameRate;//Equals to frame per second
    public List<Sprite> Frames;//images the button will animate through

    private Button _frameHolder;

    private int _currentFrameIndex;
    private Coroutine _currentFrameAnimation;

    public void Initialize(Button frameHolder)
    {
        _frameHolder = frameHolder;
    }

    public void Run(bool timeScaleDependent)
    {
        if (Frames.Count == 0)
            return;


        if (_currentFrameAnimation != null)
            _frameHolder.StopCoroutine(_currentFrameAnimation);
        _currentFrameAnimation = _frameHolder.StartCoroutine(AnimateFrames(timeScaleDependent));

        IEnumerator AnimateFrames(bool timeScaleDependent)
        {
            while (true)
            {
                if (!timeScaleDependent)
                    yield return new WaitForSecondsRealtime(1 / FrameRate);
                else
                    yield return new WaitForSeconds(1 / FrameRate);

                _currentFrameIndex++;
                _currentFrameIndex %= Frames.Count;

                //Assign the select frame image to the button's sprite state
                //To change the transition of the sprite when selected
                var spriteState = _frameHolder.spriteState;
                spriteState.selectedSprite = Frames[_currentFrameIndex];
                _frameHolder.spriteState = spriteState;
            }
        }
    }

    public void Stop()
    {
        if (_currentFrameAnimation != null)
            _frameHolder.StopCoroutine(_currentFrameAnimation);
    }

}
