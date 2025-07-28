using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SelectableElement : Button
{
    protected SelectableElementInspector _elementInspector;
    protected iDisplayeableElement _currentElement;

    public void InitItem(iDisplayeableElement element)
    {
        _currentElement = element;
        SetupElement();
    }

    public void Init(SelectableElementInspector elementInspector)
    {
        _elementInspector = elementInspector;
    }


    public virtual void SetupElement()
    {
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        //Message any visual components that this element has been selected
        BroadcastMessage("OnSelected", SendMessageOptions.DontRequireReceiver);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        //Message any visual components that this element has been deselected
        BroadcastMessage("OnDeselected", SendMessageOptions.DontRequireReceiver);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        BroadcastMessage("OnSelected", SendMessageOptions.DontRequireReceiver);
        OnSelect(eventData);
        Debug.Log("OnPointerEnter: " + name);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        BroadcastMessage("OnDeselected", SendMessageOptions.DontRequireReceiver);
        OnDeselect(eventData);
        Debug.Log("OnPointerExit: " + name);
    }
}
