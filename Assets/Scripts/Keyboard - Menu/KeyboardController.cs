using DG.Tweening;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class KeyboardController : MonoBehaviour
{
    #region Singleton
    public static KeyboardController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _menuController = MenuController.Instance;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    #endregion


    public Action<Char> OnKeyPressed;
    public RectTransform KeyReferenceTarget => _keyReferenceTarget;
    public KeyboardSettings KeyboardSettings => _keyboardSettings;

    [Header("References")]
    [SerializeField] private RectTransform _keyReferenceTarget;
    [SerializeField] private TMP_Text _resultingText;
    [SerializeField] private GameObject _keyboardHolder;

    [Header("Keyboard Settings")]
    [SerializeField] private int _maxTextLength = 20;

    [Header("Key Settings")]
    [SerializeField] private KeyboardSettings _keyboardSettings;

    [Header("Confirm Settings")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    private MenuController _menuController;
    private EventSystem _eventSystem;
    private KeyboardKey[] _keyboardKeys = new KeyboardKey[0];
    private Tween _shakeTween;
    private Action<string> _resultingStringOnConfirm;
    private Tween _moveTween;
    private Tween _indicatorLoopTween;
    private int _currentTextIndex;




    //Array of all supported keyboard characters
    private Char[] _keyboardChars = new Char[27] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P',
                                                    'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L',
                                                    'Z', 'X', 'C', 'V', 'B', 'N', 'M', ' ' // ' ' for space . '\b' for backspace
    };

    private void OnEnable()
    {
        PlayerInput.OnBackActionUI += BackAction;
        PlayerInput.OnConfirmActionUI += ConfirmAction;
        Keyboard.current.onTextInput += OnKeyPress;
    }

    private void OnDisable()
    {
        PlayerInput.OnBackActionUI -= BackAction;
        PlayerInput.OnConfirmActionUI -= ConfirmAction;
        Keyboard.current.onTextInput -= OnKeyPress;

        Clean();
    }


    private void SetupKeyboard()
    {
        _keyboardKeys = GetComponentsInChildren<KeyboardKey>(true);
        for (int i = 0; i < _keyboardKeys.Length; i++)
        {
            KeyboardKey key = _keyboardKeys[i];
            key.Initialize(this, _keyboardChars[i]);
        }
        _resultingText.maxVisibleCharacters = 0;
    }

    private void SetupConfirmation()
    {
        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(Confirm);
        _cancelButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.AddListener(Cancel);
    }


    //Caution: Is also called every repeat key when holding down the same key
    public void OnKeyPress(Char character)
    {
        // Handle backspace character
        if (character == '\b')
        {
            if (_resultingText.text.Length > 0)
            {
                _currentTextIndex--;
                _resultingText.maxVisibleCharacters = _currentTextIndex;
                _resultingText.text = _resultingText.text.Substring(0, _resultingText.text.Length - 1);
            }
        }

        // Do not allow text to exceed max length
        if (_resultingText.text.Length >= _maxTextLength)
        {
            return;
        }

        //if the character is in the keyboardChars array, type it
        if (_keyboardChars.Any(c => KeyboardUtils.CompareNormalized(c, character)))
        {
            _resultingText.text += character;
            //max visible count is updated when keyboardkey animation is compelted
        }

        // Handle space character
        if (character == ' ')
        {
            _resultingText.text += "_";
            _currentTextIndex++;
        }

        Shake();
        OnKeyPressed?.Invoke(character);
    }



    public void UpdateTextComponent()
    {
        _currentTextIndex++;
        _resultingText.maxVisibleCharacters = _currentTextIndex;
    }

    private void Shake()
    {
        _shakeTween?.Kill(); // Kill any existing shake tween
        _keyboardHolder.transform.localPosition = Vector3.zero; // Reset position before shaking
        _shakeTween = _keyboardHolder.transform.DOPunchPosition(new Vector3(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20), 0), 0.3f)
              .OnComplete(() =>
              {
                  _keyboardHolder.transform.DOLocalMove(Vector3.zero, 0.1f);
              });
    }

    private void Confirm()
    {
        _resultingStringOnConfirm?.Invoke(_resultingText.text.Replace("_", " "));
        DisableVirtualKeyboard();
    }

    private void Cancel()
    {
        _resultingText.text = string.Empty;
        DisableVirtualKeyboard();
    }


    public void InitKeyboard(EventSystem eventSystem, MenuController menuController)
    {
        _eventSystem = eventSystem;
        _menuController = menuController;

        DisableVirtualKeyboard();
    }

    /// <summary>
    /// Enables the virtual keyboard and prepares it for input
    /// </summary>
    public void EnableVirtualKeyboard(Action<string> resultingStringOnConfirm)
    {
        //assign references
        _resultingStringOnConfirm = resultingStringOnConfirm;

        //setup
        _menuController.BlockMenu(this, true);
        SetupKeyboard();
        SetupConfirmation();

        //animate
        _moveTween?.Kill();
        _keyboardHolder.SetActive(true);
        _moveTween = transform.DOLocalMoveY(0, .2f);

        //navigation
        //If using a keyboard only allow typing with the keyboard
        if (ReflectCurrentInput.CurrentInputType != InputType.MouseAndKeyboard)
            _eventSystem.SetSelectedGameObject(null); // Clear previous selection  
        //if not using a keyboard select object for navigation
        else
            _eventSystem.SetSelectedGameObject(_keyboardKeys[0].gameObject);

        _currentTextIndex = 0;

        _indicatorLoopTween = _keyReferenceTarget.GetComponent<TMP_Text>().DOFade(0, 0.1f);
        _indicatorLoopTween.SetLoops(-1, LoopType.Yoyo); // Loop the fade effect indefinitely
    }


    public void DisableVirtualKeyboard()
    {
        _moveTween?.Kill();
        _indicatorLoopTween?.Kill();
        transform.localPosition = Vector3.zero; // Reset position before moving out
        _moveTween = transform.DOLocalMoveY(-900, .2f);
        _moveTween.OnComplete(() => { Clean(); });

        _menuController.BlockMenu(this, false);
    }

    private void Clean()
    {
        _keyboardHolder?.SetActive(false);
        _resultingText.text = string.Empty;
        _resultingStringOnConfirm = null;

        _moveTween?.Kill();
        _shakeTween?.Kill();

        foreach (var key in _keyboardKeys)
        {
            key.Uninitialize();
        }
    }

    /// <summary>
    /// Calling backspace from specific input actions on controllers
    /// </summary>
    private void BackAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnKeyPress('\b'); // Trigger backspace action
        }
    }

    private void ConfirmAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Confirm();
    }

}

[System.Serializable]
public class KeyboardSettings
{
    public float JumpHeight;
    public float JumpDuration;
    public AnimationCurve JumpAnimationCurve;
}

public static class KeyboardUtils
{
    public static char NormalizeChar(this char c)
    {
        string normalized = c.ToString().ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD);
        foreach (char ch in normalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                return ch;
        }
        return c;
    }

    public static bool CompareNormalized(Char char1, Char char2)
    {
        return char1.NormalizeChar() == char2.NormalizeChar();
    }

    public static KeyControl GetKeyControlFromChar(Char c)
    {
        c = RemoveDiacritics(char.ToLowerInvariant(c));
        switch (c)
        {
            case 'a': return Keyboard.current.aKey;
            case 'b': return Keyboard.current.bKey;
            case 'c': return Keyboard.current.cKey;
            case 'd': return Keyboard.current.dKey;
            case 'e': return Keyboard.current.eKey;
            case 'f': return Keyboard.current.fKey;
            case 'g': return Keyboard.current.gKey;
            case 'h': return Keyboard.current.hKey;
            case 'i': return Keyboard.current.iKey;
            case 'j': return Keyboard.current.jKey;
            case 'k': return Keyboard.current.kKey;
            case 'l': return Keyboard.current.lKey;
            case 'm': return Keyboard.current.mKey;
            case 'n': return Keyboard.current.nKey;
            case 'o': return Keyboard.current.oKey;
            case 'p': return Keyboard.current.pKey;
            case 'q': return Keyboard.current.qKey;
            case 'r': return Keyboard.current.rKey;
            case 's': return Keyboard.current.sKey;
            case 't': return Keyboard.current.tKey;
            case 'u': return Keyboard.current.uKey;
            case 'v': return Keyboard.current.vKey;
            case 'w': return Keyboard.current.wKey;
            case 'x': return Keyboard.current.xKey;
            case 'y': return Keyboard.current.yKey;
            case 'z': return Keyboard.current.zKey;
            case ' ': return Keyboard.current.spaceKey;
            case '\b': return Keyboard.current.backspaceKey;
            default: return null;
        }
    }

    private static char RemoveDiacritics(char c)
    {
        string normalized = c.ToString().Normalize(NormalizationForm.FormD);
        foreach (char ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                return ch;
        }
        return c;
    }
}
