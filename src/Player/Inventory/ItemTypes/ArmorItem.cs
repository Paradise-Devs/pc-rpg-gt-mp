using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Player.Inventory.Classes;
using pcrpg.src.Player.Inventory.Interfaces;

namespace pcrpg.src.Player.Inventory.ItemTypes
{
    public class ArmorItem : BaseItem, IDroppable
    {
        public int Value { get; set; }
        public WorldModel DropModel { get; set; }

        public ArmorItem(string name, string description, int stackSize, int armorValue, WorldModel dropModel) : base(name, description, stackSize)
        {
            Value = armorValue;
            DropModel = dropModel;

            Category = "Colete";
            Rarity = "rare";
            Icon = "clothes-hoodie";
            Usable = true;
            Tradable = true;
            NeedConfirmation = false;
        }

        public override bool Use(Client player)
        {
            if (player.armor == 100)
            {
                player.sendChatMessage("You have full armor.");
                return false;
            }

            player.sendChatMessage($"Used armor. (+{Value})");
            player.armor = ((player.armor + Value > 100) ? 100 : player.armor + Value);
            return true;
        }
    }
}