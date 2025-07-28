using System.Collections.Generic;
using UnityEngine;

public class InventoryItemInjection : MonoBehaviour
{
    //Inject a list of items into an inventory manager for debugging

    [SerializeField] private List<Item> _itemsToInject = new();
    [SerializeField] private List<ItemAmount> _amoutsToInject = new();
    private InventoryManager _inventoryManager;

    private void Start()
    {
        _inventoryManager = InventoryManager.Instance;
        InjectItems();
    }

    public void InjectItems()
    {
        foreach (var item in _itemsToInject)
        {
            _inventoryManager.AddItem(item);
        }

        foreach (var item in _amoutsToInject)
        {
            _inventoryManager.AddItem(item.Item, item.Amount);
        }
    }
}
