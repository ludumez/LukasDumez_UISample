using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //Class used to hold a list of current held items and the amount of said items
    //Display and stacking logic is handeled by each UI component on its own

    public int MaxInventorySize => _maxInventorySize;
    [SerializeField] private int _maxInventorySize;

    public Dictionary<Item, int> ItemList => _itemList;

    private Dictionary<Item, int> _itemList = new();

    #region singleton
    public static InventoryManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    public int GetItemAmount(Item item)
    {
        if (_itemList.ContainsKey(item))
            return _itemList[item];
        else
            return 0;
    }

    public void AddItem(Item item, int amount = 1)
    {
        if (!_itemList.ContainsKey(item))
            _itemList.Add(item, 0);

        _itemList[item] += amount;
    }

    //Removes an item from the inventory, by default removes only one
    //Returns a bool if the item can be removed from said list
    public bool RemoveItem(Item item, int amount = 1)
    {
        if (_itemList.ContainsKey(item) && _itemList[item] >= amount)
        {
            _itemList[item] -= amount;
            if (_itemList[item] == 0)
                _itemList.Remove(item);
        }
        return false;
    }

    public Dictionary<T, int> GetItemsOfType<T>() where T : Item
    {
        Dictionary<T, int> result = new Dictionary<T, int>();
        foreach (var item in _itemList)
        {
            if (item.Key is T)
                result.Add(item.Key as T, item.Value);
        }
        return result;
    }

}
