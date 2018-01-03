using GrandTheftMultiplayer.Server.Elements;

namespace pcrpg.src.Player.Inventory.ItemTypes
{
    public class BaseItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int StackSize { get; set; }

        public string Rarity { get; set; }
        public string Category { get; set; }
        public string Icon { get; set; }
        public bool Usable { get; set; }
        public bool Tradable { get; set; }
        public bool NeedConfirmation { get; set; }

        public BaseItem(string name, string description, int stackSize)
        {
            Name = name;
            Description = description;
            StackSize = stackSize;
        }

        public virtual bool Use(Client player)
        {
            return true;
        }
    }
}
