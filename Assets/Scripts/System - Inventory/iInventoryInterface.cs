public interface iInventoryInterface
{
    //Interface used to initialize all types of inventories with needed managers

    //Initi is called before any inventory is being used, after the inventory manager has been initailized
    public void InitInventory(InventoryManager inventoryManager);
    public InventoryManager InventoryManager { get; }
    public MenuController MenuController { get; }
}
