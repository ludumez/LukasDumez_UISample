using System;
using UnityEngine;

public class BaitSelectionManager : MonoBehaviour
{
    public static BaitSelectionManager Instance;
    public static Action<Item> OnItemSelected;

    public ItemBait CurrentSelectedBait => _currentSelectedBait;
    private ItemBait _currentSelectedBait;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SelectItem(Item item)
    {
        if (item is ItemBait)
        {
            OnItemSelected?.Invoke(item);
            _currentSelectedBait = item as ItemBait;
        }
    }
}
