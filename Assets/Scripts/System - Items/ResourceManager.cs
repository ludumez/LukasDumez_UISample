using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    #region Singleton
    public static ResourceManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    #endregion

    [SerializeField] private Item _explorationResource;
    [SerializeField] private Item _fishResource;

    public int ResourceAmount => _resourceAmount;
    private int _resourceAmount;

    Dictionary<Item, int> _itemDictionary;

    public void AddResource(int amount = 1)
    {
        _resourceAmount += amount;
    }

    public void RemoveResource(int amout = 1)
    {
        _resourceAmount -= amout;
        _resourceAmount = Mathf.Max(_resourceAmount, 0);//Clamp value to 0
    }

}
