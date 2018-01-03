using pcrpg.src.Player.Inventory.Interfaces;

namespace pcrpg.src.Player.Inventory.Classes
{
    public class InventoryItemData
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int StackSize { get; private set; }
        public int Quantity { get; private set; }
        public bool IsDroppable { get; private set; }

        public string Rarity { get; set; }
        public string Category { get; set; }
        public string Icon { get; set; }
        public bool Usable { get; set; }
        public bool Tradable { get; set; }
        public bool NeedConfirmation { get; set; }

        public InventoryItemData(InventoryItem item)
        {
            Name = item.Item.Name;
            Description = item.Item.Description;
            StackSize = item.Item.StackSize;
            Quantity = item.Quantity;
            IsDroppable = (item.Item is IDroppable);

            Rarity = item.Item.Rarity;
            Category = item.Item.Category;
            Icon = item.Item.Icon;
            Usable = item.Item.Usable;
            Tradable = item.Item.Tradable;
            NeedConfirmation = item.Item.NeedConfirmation;
        }
    }
}
