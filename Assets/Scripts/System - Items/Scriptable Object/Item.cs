using UnityEngine;

[CreateAssetMenu(menuName = "Items")]
public class Item : ScriptableObject, iDisplayeableElement
{
    //Scriptable object that just holds the icon, name and description of the item
    //Used to be displayed in the inventory menus
    //Can be obtained by interacting with objects
    //Can be saved by saving a value for amount and asset path

    [SerializeField] public string _itemName;
    [SerializeField, TextArea(1, 10)] public string _itemDescription;
    [SerializeField] public Sprite _itemSprite;


    string iDisplayeableElement.ElementName => _itemName;

    string iDisplayeableElement.ElementDespcription => _itemDescription;

    Sprite iDisplayeableElement.ElementSprite => _itemSprite;

    //For saving
    public string AssetPath;
#if UNITY_EDITOR
    private void OnValidate()
    {

    }
#endif
}
