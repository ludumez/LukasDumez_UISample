using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    //Custom class that just disables the drag function of a scroll rect
    public override void OnDrag(PointerEventData eventData)
    {
        //base.OnDrag(eventData);
    }
}
