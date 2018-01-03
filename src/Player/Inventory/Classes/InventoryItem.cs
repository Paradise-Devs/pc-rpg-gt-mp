using Newtonsoft.Json;
using pcrpg.src.Player.Inventory.ItemTypes;

namespace pcrpg.src.Player.Inventory.Classes
{
    public class InventoryItem
    {
        public ItemID ID { get; private set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public BaseItem Item { get; private set; }

        public InventoryItem(ItemID ID, int quantity)
        {
            if (!ItemDefinitions.ItemDictionary.ContainsKey(ID)) return;

            this.ID = ID;
            Quantity = quantity;
            Item = ItemDefinitions.ItemDictionary[ID];
        }
    }
}