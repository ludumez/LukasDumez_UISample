using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NameSelectionElement : SelectableElement
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private KeyboardController _keyboardController;

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        _keyboardController.EnableVirtualKeyboard((resultText) => _nameText.text = resultText);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        _keyboardController.EnableVirtualKeyboard((resultText) => _nameText.text = resultText);
    }
}
