using UnityEngine;

public interface iDisplayeableElement
{
    //Interface for elements that can be selected to display information in a menu
    // Provides the name, description and sprite of the element
    public string ElementName { get; }
    public string ElementDespcription { get; }
    public Sprite ElementSprite { get; }

}
