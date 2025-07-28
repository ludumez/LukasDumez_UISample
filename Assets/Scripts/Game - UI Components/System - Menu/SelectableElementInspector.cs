using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableElementInspector : MonoBehaviour
{
    [SerializeField] private TMP_Text _elementName;
    [SerializeField] private TMP_Text _elementDescription;
    [SerializeField] private Image _elementIcon;

    [SerializeField] private Sprite _selectedElementSprite;
    [SerializeField] private Sprite _defaultElementDisplaySprite;
    [SerializeField] private Image _elementBorderImage;

    [Header("Input Feedback")]
    [SerializeField] private TMP_Text _inputTextElement;


    /// <summary>
    /// Display the current item name and description
    /// </summary>
    /// <param name="item">The item we are looking at</param>
    /// <param name="isCurrentSelected">Indicates that we are looking at the current selected item to display a different sprite background</param>
    public void DisplayElement(iDisplayeableElement item, bool isCurrentSelected)
    {
        //Animate that we are selecting an item that can be selected
        //If there is no text component referenced we asume we can't select any element
        if (_inputTextElement != null)
        {
            if (isCurrentSelected)
                _inputTextElement.StopAnimation();
            else
                _inputTextElement.Animate();
        }


        if (item == null)
        {
            DisplayEmpty();
            return;
        }
        _elementIcon.enabled = true;

        _elementName.SetText(item.ElementName);
        _elementDescription.SetText(item.ElementDespcription);
        _elementIcon.sprite = item.ElementSprite;

        _elementBorderImage.sprite = isCurrentSelected ? _selectedElementSprite : _defaultElementDisplaySprite;

    }

    private void DisplayEmpty()
    {
        _elementName.SetText("");
        _elementDescription.SetText("");
        _elementIcon.sprite = null;
        _elementIcon.enabled = false;
        _elementBorderImage.sprite = _defaultElementDisplaySprite;

        if (_inputTextElement != null)
            _inputTextElement.StopAnimation();

    }
}
