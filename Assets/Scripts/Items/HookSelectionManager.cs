using System;
using UnityEngine;

public class HookSelectionManager : MonoBehaviour
{
    public static HookSelectionManager Instance;
    public static Action<ItemHook> OnHookSelected;
    public ItemHook CurrentHookItem => _currentHookItem;
    [SerializeField] private ItemHook _currentHookItem;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SelectItem(Item item)
    {
        if (item is ItemHook itemHook)
        {
            _currentHookItem = itemHook;
            OnHookSelected?.Invoke(itemHook);
        }
    }
}
